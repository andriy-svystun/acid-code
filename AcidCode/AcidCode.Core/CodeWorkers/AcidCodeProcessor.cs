using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcidCode.Db;
using AcidCode.Common;

namespace AcidCode.Core
{
    public class AcidCodeProcessor : IAcidCodeProcessor
    {
        private readonly IAcidCodeRepository _codeRepository;

        public AcidCodeProcessor(IAcidCodeRepository codeRepository)
        {
            _codeRepository = codeRepository;
        }

        public async Task<IAcidCodeCompiler> GetCodeCompilerFromText(string codetext)
        {
            AcidCodeCompiler acidCodeCompiler = new AcidCodeCompiler(codetext, _codeRepository.GetSettings());

            await acidCodeCompiler.ParseCode();

            return acidCodeCompiler;
        }

        public Task<IAcidCodeRunner> GetCodeRunner(CodeItem codeItem)
        {
            throw new NotImplementedException();
        }

        public async Task<IAcidCodeRunner> GetCodeRunner(IAcidCodeCompiler codeCompiler)
        {
            codeCompiler = codeCompiler ??  throw new ArgumentNullException(nameof(codeCompiler));

            if (!codeCompiler.IsCodeCompiled)
            {
                throw new ApplicationException("Compile code first");
            }

            AcidCodeRunner codeRunner = new AcidCodeRunner(codeCompiler, await _codeRepository.GetSettingsAsync());

            return codeRunner;
        }

        public CodeParsingResult ParseCodeText(string codetext)
        {
            throw new NotImplementedException();
        }

        public async Task<CodeItem> SaveCompiledCode(IAcidCodeCompiler codeCompiler)
        {
            codeCompiler = codeCompiler ?? throw new ArgumentNullException(nameof(codeCompiler));

            if (!codeCompiler.IsCodeCompiled)
            {
                throw new ApplicationException("Compile code before saving");
            }

            CodeItem codeItem = new CodeItem();

            codeItem.CodeText = codeCompiler.CodeText;
            codeItem.EntryPoint = codeCompiler.EntryPoint;

            await _codeRepository.SaveCodeItemAsync(codeItem);

            return codeItem;
        }
    }
}
