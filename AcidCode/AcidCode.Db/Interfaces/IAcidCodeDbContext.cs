using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using AcidCode.Common;

namespace AcidCode.Db
{
    public interface IAcidCodeDbContext : IDisposable
    {
        DbSet<CodeItem> CodeItems { get; set; }
        DbSet<GeneralSiteSettings> GeneralSiteSettings { get; set;}

        void SaveChanges();
        Task SaveChangesAsync();
    }
}
