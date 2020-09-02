using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcidCode.Core
{
    public interface IAcidCodeCompiler : IDisposable
    {
        string CodeText { get; }
        string EntryPoint { get; }
        MemoryStream CompiledCode { get; }

        SyntaxTree ParsedSyntaxTree { get; }

        bool IsCodeCompiled { get; }
        string CompilationErrors { get; }

        Task CompileCodeAsync();
        void CompileCode();
    }
}
