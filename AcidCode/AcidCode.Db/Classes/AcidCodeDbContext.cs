using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using AcidCode.Common;

namespace AcidCode.Db
{
    public class AcidCodeDbContext : IdentityDbContext<AcidCodeUser>, IAcidCodeDbContext
    {
        public AcidCodeDbContext() : base(AcidCodeConstants.DATABASE_NAME)
        {
            Database.SetInitializer(new AcidCodeDbContextInitializer());
        }

        public DbSet<CodeItem> CodeItems { get; set; }
        public DbSet<GeneralSiteSettings> GeneralSiteSettings { get; set; }

        void IAcidCodeDbContext.SaveChanges()
        {
            SaveChanges();
        }

        async Task IAcidCodeDbContext.SaveChangesAsync()
        {
            await SaveChangesAsync();
        }
    }
}
