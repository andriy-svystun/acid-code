using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;
using AcidCode.Core.Helpers;
using AcidCode.Common;

namespace AcidCode.Core
{
    public class AcidCodeRunner : IAcidCodeRunner
    {
        #region Private fields
        private readonly IAcidCodeCompiler _codeCompiler;
        private readonly GeneralSiteSettings _siteSettings;
        private string _codeText;
        private SyntaxTree _codeSyntaxTree;
        private string _codeOutput;
        private string _lastErrors;
        private TimeSpan _executionTime;
        #endregion


        #region Constructors
        public AcidCodeRunner()
        {
            _codeSyntaxTree = null;
        }

        public AcidCodeRunner(IAcidCodeCompiler codeCompiler)
        {
            _codeCompiler = codeCompiler ?? throw new ArgumentNullException(nameof(codeCompiler));
            _siteSettings = new GeneralSiteSettings();
        }

        public AcidCodeRunner(IAcidCodeCompiler codeCompiler, GeneralSiteSettings siteSettings) : this(codeCompiler)
        {
            _siteSettings = siteSettings ?? throw new ArgumentNullException(nameof(siteSettings));
        }
        #endregion

        #region Public properties
        public string CodeOutput => _codeOutput;

        public string LastErrors => _lastErrors;

        public TimeSpan ExecutionTime => _executionTime;
        #endregion

        #region Interface methods
        public async Task RunCodeAsync()
        {
            ResetStatus();

            if (!_codeCompiler.IsCodeCompiled)
                throw new ApplicationException("Compile code first");

            var ms = _codeCompiler.CompiledCode;
            ms.Seek(0, SeekOrigin.Begin);
            Assembly assembly = Assembly.Load(ms.ToArray());

            Type type = assembly.GetType(_codeCompiler.EntryPoint);

            Task<CodeExecutionresult> executionresult = GetExecutionResultAsync(type);
            System.Diagnostics.Stopwatch exectimesw = new Stopwatch();
            TimeSpan exectime = TimeSpan.Zero;

            try
            {
                if (executionresult == await Task.WhenAny(executionresult, Task.Delay(_siteSettings.RunningTime == TimeSpan.Zero ? TimeSpan.FromMilliseconds(-1)
                                                                                      : _siteSettings.RunningTime)).ContinueWith(t => { exectime = exectimesw.Elapsed; return t.Result; }))
                {
                    _executionTime = executionresult.Result.ExecutionTime;
                    _codeOutput = executionresult.Result.CodeOutput;
                }
                else
                {
                    _lastErrors = $"Running time {exectime.ToString()} exceeded allowed timeout";
                }
            }
            catch(AcidCodeInternalExeption ex)
            {
                _lastErrors = $"Exeptions thrown in your code: {ex.CodeExeptionMessage}";
            }
        }

        public void RunCode()
        {
            ResetStatus();

            if (!_codeCompiler.IsCodeCompiled)
                throw new ApplicationException("Compile code first");

            var ms = _codeCompiler.CompiledCode;


            ms.Seek(0, SeekOrigin.Begin);
            Assembly assembly = Assembly.Load(ms.ToArray());

            Type type = assembly.GetType(_codeCompiler.EntryPoint);

            CodeExecutionresult executionresult = GetExecutionResult(type);

            _executionTime = executionresult.ExecutionTime;
            _codeOutput = executionresult.CodeOutput;

            /*TextWriter tmp = Console.Out;
            var codeoutput = new StringWriter();

            Console.SetOut(codeoutput);

            object obj = Activator.CreateInstance(type);

            // Calculationg execution time. Another solution is - to use https://benchmarkdotnet.org/index.html
            System.Diagnostics.Stopwatch sw = new Stopwatch();
            sw.Start();

            type.InvokeMember("Main",
                BindingFlags.Default | BindingFlags.InvokeMethod,
                null,
                obj,
                null);

            sw.Stop();

            _executionTime = sw.Elapsed;

            Console.SetOut(tmp);

            _codeOutput = codeoutput.ToString();*/

        }
        #endregion

        #region Private methods
        private async Task<CodeExecutionresult> GetExecutionResultAsync(Type type)
        {
            return await Task.Run(() => { return GetExecutionResult(type); });
        }
        
        private CodeExecutionresult GetExecutionResult(Type type)
        {
            TextWriter tmp = Console.Out;
            var codeoutput = new StringWriter();
            System.Diagnostics.Stopwatch sw = new Stopwatch();

            try
            {
                Console.SetOut(codeoutput);

                object obj = Activator.CreateInstance(type);

                // Calculationg execution time. Another solution is - to use https://benchmarkdotnet.org/index.html
                sw.Start();

                try
                {
                    type.InvokeMember("Main",
                        BindingFlags.Default | BindingFlags.InvokeMethod,
                        null,
                        obj,
                        null);
                }
                catch(Exception ex)
                {
                    throw new AcidCodeInternalExeption(ex);
                }
                sw.Stop();

            }
            finally
            {
                Console.SetOut(tmp);
            }
            return new CodeExecutionresult(sw.Elapsed, codeoutput.ToString());
        }

        private void ResetStatus()
        {
            _codeOutput = String.Empty;
            _lastErrors = String.Empty;
            _executionTime = TimeSpan.Zero;
        }

        private string GetEntryPoint(CompilationUnitSyntax rootnode)
        {
            string result = "";

            if (rootnode.Members.Count == 0)
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
        #endregion

        #region Obsolate
        [Obsolete()]
        public void SetCodeText(string codetext)
        {
            if (String.IsNullOrWhiteSpace(codetext))
                throw new ArgumentException(nameof(codetext));

            _codeText = codetext;
        }

        [Obsolete()]
        public async Task<bool> CompileAndRunCodeAsync()
        {
            bool retValue = true;

            ResetStatus();

            if (_codeSyntaxTree == null)
            {
                _codeSyntaxTree = await SyntaxTreeHelpers.ParseTextTaskAsync(_codeText);
            }

            string baseType = GetEntryPoint((CompilationUnitSyntax)_codeSyntaxTree.GetRoot());

            if (String.IsNullOrEmpty(baseType))
                throw new ApplicationException("Cannot found entry point for code execution. Add static method Main to the first class");

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

            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                if (!result.Success)
                {
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    var errorsTmp = new StringBuilder();
                    foreach (Diagnostic diagnostic in failures)
                    {
                        errorsTmp.AppendLine($"{diagnostic.Id}: {diagnostic.GetMessage()}");
                    }

                    _lastErrors = errorsTmp.ToString();

                    retValue = false;
                }
                else
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    Assembly assembly = Assembly.Load(ms.ToArray());

                    Type type = assembly.GetType(baseType);

                    TextWriter tmp = Console.Out;
                    var codeoutput = new StringWriter();
                    Console.SetOut(codeoutput);

                    object obj = Activator.CreateInstance(type);

                    // Calculationg execution time. Another solution is - to use https://benchmarkdotnet.org/index.html
                    System.Diagnostics.Stopwatch sw = new Stopwatch();
                    sw.Start();

                    type.InvokeMember("Main",
                        BindingFlags.Default | BindingFlags.InvokeMethod,
                        null,
                        obj,
                        null);

                    sw.Stop();

                    _executionTime = sw.Elapsed;

                    Console.SetOut(tmp);

                    _codeOutput = codeoutput.ToString();
                }

            }

            return retValue;
        }

        #endregion

    }

    class CodeExecutionresult
    {
        private TimeSpan _executionTime;
        private string _codeOutput;

        public CodeExecutionresult(TimeSpan executionTime, string codeOutput)
        {
            _executionTime = executionTime; _codeOutput = codeOutput;
        }

        public TimeSpan ExecutionTime { get => _executionTime; }
        public string CodeOutput { get => _codeOutput; }
    }
}
