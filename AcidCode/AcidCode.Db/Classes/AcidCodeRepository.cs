using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcidCode.Common;

namespace AcidCode.Db
{
    public class AcidCodeRepository : IAcidCodeRepository
    {
        private readonly IAcidCodeDbContext _dbContext;

        public AcidCodeRepository(IAcidCodeDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IEnumerable<CodeItem> CodeItems => _dbContext.CodeItems;

        private IEnumerable<GeneralSiteSettings> GeneralSiteSettings => _dbContext.GeneralSiteSettings;

        public CodeItem GetCodeItem(int id)
        {
            return _dbContext.CodeItems.Find(id);
        }

        public async Task<CodeItem> GetCodeItemAsync(int id)
        {
            return await _dbContext.CodeItems.FindAsync(id);
        }

        public GeneralSiteSettings GetSettings()
        {
            return _dbContext.GeneralSiteSettings.SingleOrDefault() ?? new GeneralSiteSettings { Settings = Guid.NewGuid().ToString() };
        }

        public async Task<GeneralSiteSettings> GetSettingsAsync()
        {
            var settings = await _dbContext.GeneralSiteSettings.SingleOrDefaultAsync();
            return settings ?? new GeneralSiteSettings { Settings = Guid.NewGuid().ToString() };
        }

        public void SaveCodeItem(CodeItem codeItem)
        {
            codeItem = codeItem ?? throw new ArgumentNullException(nameof(codeItem));

            if (codeItem.Id == 0)
            {
                _dbContext.CodeItems.Add(codeItem);
            }
            else
            {
                var dbEntry = _dbContext.CodeItems.Find(codeItem.Id);

                if (dbEntry != null)
                {
                    dbEntry.CodeText = codeItem.CodeText;
                    dbEntry.EntryPoint = codeItem.EntryPoint;
                }
            }

            _dbContext.SaveChanges();
        }

        public async Task SaveCodeItemAsync(CodeItem codeItem)
        {
            codeItem = codeItem ?? throw new ArgumentNullException(nameof(codeItem));

            if (codeItem.Id == 0)
            {
                _dbContext.CodeItems.Add(codeItem);
            }
            else
            {
                var dbEntry = await _dbContext.CodeItems.FindAsync(codeItem.Id);

                if (dbEntry != null)
                {
                    dbEntry.CodeText = codeItem.CodeText;
                    dbEntry.EntryPoint = codeItem.EntryPoint;
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        public void SaveSettings(GeneralSiteSettings settings)
        {
            settings = settings ?? throw new ArgumentNullException(nameof(settings));

            var dbEntry = _dbContext.GeneralSiteSettings.SingleOrDefault();

            if (dbEntry == null)
                _dbContext.GeneralSiteSettings.Add(settings);
            else
            {
                dbEntry.CompilationTime = settings.CompilationTime;
                dbEntry.RunningTime = settings.RunningTime;
            }

            _dbContext.SaveChanges();
        }

        public async Task SaveSettingsAsync(GeneralSiteSettings settings)
        {
            settings = settings ?? throw new ArgumentNullException(nameof(settings));

            var dbEntry = await _dbContext.GeneralSiteSettings.SingleOrDefaultAsync();

            if (dbEntry == null)
            {
                _dbContext.GeneralSiteSettings.Add(settings);
            }
            else
            {
                dbEntry.CompilationTime = settings.CompilationTime;
                dbEntry.RunningTime = settings.RunningTime;
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
