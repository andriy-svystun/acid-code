using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcidCode.Common;

namespace AcidCode.Db
{
    public interface IAcidCodeRepository
    {
        IEnumerable<CodeItem> CodeItems { get; }

        Task SaveCodeItemAsync(CodeItem codeItem);
        void SaveCodeItem(CodeItem codeItem);
        CodeItem GetCodeItem(int id);
        Task<CodeItem> GetCodeItemAsync(int id);

        Task SaveSettingsAsync(GeneralSiteSettings settings);
        void SaveSettings(GeneralSiteSettings settings);
        GeneralSiteSettings GetSettings();
        Task<GeneralSiteSettings> GetSettingsAsync();

    }
}
