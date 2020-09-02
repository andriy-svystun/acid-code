using AcidCode.Core.Helpers;
using AcidCode.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace AcidCode.Core
{
    public class AcidCodeCompiler : IAcidCodeCompiler
    {
        #region Private fields
        readonly string _codetext;
        private readonly GeneralSiteSettings _siteSettings;
        private SyntaxTree _codeSyntaxTree;
        private string _baseType;
        private string _entryPoint;
        private bool _isCodeCompiled;
        private MemoryStream _compiledCode;
        private string _compilationErrors;
        #endregion

        #region Constructors
        public AcidCodeCompiler(string codetext)
        {
            _codetext = codetext;
            _isCodeCompiled = false;
            _compiledCode = null;
            _compilationErrors = String.Empty;
            _siteSettings = new GeneralSiteSettings();
        }
        

        public AcidCodeCompiler(string codetext, GeneralSiteSettings siteSettings) : this(codetext)
        {
            _siteSettings = siteSettings ?? throw new ArgumentNullException(nameof(siteSettings));
        }
        #endregion

        #region Public properties
        public string BaseType => _baseType;

        public SyntaxTree ParsedSyntaxTree => _codeSyntaxTree;

        public string EntryPoint
        {
            get
            {
                if (String.IsNullOrEmpty(_entryPoint))
                {
                    _entryPoint = GetEntryPoint((CompilationUnitSyntax)_codeSyntaxTree.GetRoot());
                }

                return _entryPoint;
            }
        }

        public string CodeText => _codetext;

        public MemoryStream CompiledCode => _compiledCode;

        public bool IsCodeCompiled => _isCodeCompiled;

        public string CompilationErrors => _compilationErrors;
        #endregion

        #region Interface methods
        public async Task CompileCodeAsync()
        {
            if (_isCodeCompiled)
                return;

            Task<EmitResult> compilerResult = GetCompiledResultAsync();

            if (compilerResult == await Task.WhenAny(compilerResult, Task.Delay(_siteSettings.CompilationTime == TimeSpan.Zero ? TimeSpan.FromMilliseconds(-1) : _siteSettings.CompilationTime)))
            {
                if (compilerResult.Result.Success)
                {
                    _isCodeCompiled = true;
                    _compilationErrors = String.Empty;
                }
                else
                {
                    _isCodeCompiled = false;
                    SetCompilationErrors(compilerResult.Result);
                }
            }
            else
            {
                _isCodeCompiled = false;
                _compilationErrors = "Compilation exceeded allowed timeout";
            }
        }

        public void CompileCode()
        {
            if (_isCodeCompiled)
                return;

            var compilation = GetAssemblyCompilation();

            _compiledCode = new MemoryStream();

            EmitResult result = compilation.Emit(_compiledCode);

            if (result.Success)
            {
                _isCodeCompiled = true;
                _compilationErrors = String.Empty;
            }
            else
            {
                _isCodeCompiled = false;

                SetCompilationErrors(result);
            }

        }
        #endregion


        #region Class methods
        public async Task<bool> ParseCode()
        {
            bool retValue = true;

            if (_codeSyntaxTree is null)
            {
                _codeSyntaxTree = await SyntaxTreeHelpers.ParseTextTaskAsync(_codetext);

                _baseType = GetEntryPoint((CompilationUnitSyntax)_codeSyntaxTree.GetRoot());

                if (String.IsNullOrEmpty(_baseType))
                    throw new ApplicationException("Cannot found entry point for code execution. Add static method Main to the first class");

                _entryPoint = GetEntryPoint((CompilationUnitSyntax)_codeSyntaxTree.GetRoot());
            }

            return retValue;
        }
        #endregion

        #region Private methods
        private async Task<CSharpCompilation> GetAssemblyCompilationAsync()
        {
            return await Task.Run(() => { return GetAssemblyCompilation(); }  );
        }

        private CSharpCompilation GetAssemblyCompilation()
        {
            string assemblyName = Path.GetRandomFileName();

            MetadataReference[] references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location)
            };

            var compilation = CSharpCompilation.Create(assemblyName,
                                                        new[] { _codeSyntaxTree },
                                                        references,
                                                        new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            return compilation;
        }

        private async Task<EmitResult> GetCompiledResultAsync()
        {
            var compilation = await GetAssemblyCompilationAsync();

            _compiledCode = new MemoryStream();

            EmitResult result = compilation.Emit(_compiledCode);

            return await Task.Run(() => { return compilation.Emit(_compiledCode); });

        }

        private string GetEntryPoint(CompilationUnitSyntax rootnode)
        {
            string result = "";

            if (rootnode.Members.Count == 0)
                throw new ApplicationException("Error parsing soure code");

            if (!(rootnode.Members[0] is NamespaceDeclarationSyntax))
                throw new ApplicationException("Error parsing soure code");

            NamespaceDeclarationSyntax rootNameSpace = (NamespaceDeclarationSyntax)rootnode.Members[0];

            if (rootNameSpace is null)
                return result;

            var entryPointMethod = rootNameSpace.DescendantNodes().OfType<MethodDeclarationSyntax>().Where(n => n.Identifier.Text.Equals("Main")).FirstOrDefault();

            if (entryPointMethod is null)
                return result;

            var entryType = entryPointMethod.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();

            if (entryType is null)
                return result;

            IdentifierNameSyntax name = (IdentifierNameSyntax)rootNameSpace.Name;

            result = String.Concat(name.Identifier.Text, ".", entryType.Identifier.Text);

            return result;
        }

        private void SetCompilationErrors(EmitResult emitResult)
        {
            IEnumerable<Diagnostic> failures = emitResult.Diagnostics.Where(diagnostic =>
                                    diagnostic.IsWarningAsError ||
                                    diagnostic.Severity == DiagnosticSeverity.Error);

            var errorsTmp = new StringBuilder();
            foreach (Diagnostic diagnostic in failures)
            {
                errorsTmp.AppendLine($"{diagnostic.Id}: {diagnostic.GetMessage()}");
            }

            _compilationErrors = errorsTmp.ToString();
        }
        #endregion

        #region IDisposable implementation
        bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                    _compiledCode?.Dispose();
            }

            disposed = true;
        }
        #endregion
    }
}
