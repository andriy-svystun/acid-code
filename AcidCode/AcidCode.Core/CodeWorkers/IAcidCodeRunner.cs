using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcidCode.Core
{
    public interface IAcidCodeRunner
    {
        string CodeOutput { get; }
        string LastErrors { get; }
        TimeSpan ExecutionTime { get; }

        [Obsolete()]
        void SetCodeText(string codetext);

        [Obsolete()]
        Task<bool> CompileAndRunCodeAsync();

        void RunCode();

        Task RunCodeAsync();
    }
}
