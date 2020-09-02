using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcidCode.Common;

namespace AcidCode.Core
{
    public interface IAcidCodeProcessor
    {
        Task<IAcidCodeCompiler> GetCodeCompilerFromText(string codetext);

        Task<CodeItem> SaveCompiledCode(IAcidCodeCompiler codeCompiler);

        Task<IAcidCodeRunner> GetCodeRunner(CodeItem codeItem);

        Task<IAcidCodeRunner> GetCodeRunner(IAcidCodeCompiler codeCompiler);
    }
}
