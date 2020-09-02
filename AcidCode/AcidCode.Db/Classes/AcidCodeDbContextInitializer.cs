using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using AcidCode.Common;

namespace AcidCode.Db
{
    class AcidCodeDbContextInitializer : CreateDatabaseIfNotExists<AcidCodeDbContext>
    {
        protected override void Seed(AcidCodeDbContext context)
        {
            var usermanager = new UserManager<AcidCodeUser>(new AcidCodeUserStore(context));
            var rolemanager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            var result = rolemanager.Create(new IdentityRole(AcidCodeConstants.ADMINISTRATORS_ROLE));
            if (!result.Succeeded)
                throw new ApplicationException($"Cannot create role {AcidCodeConstants.ADMINISTRATORS_ROLE}");

            result = rolemanager.Create(new IdentityRole(AcidCodeConstants.USERS_ROLE));
            if (!result.Succeeded)
                throw new ApplicationException($"Cannot create role {AcidCodeConstants.USERS_ROLE}");


            result = usermanager.Create(new AcidCodeUser { UserName = AcidCodeConstants.ADMIN_ACCOUNT, Email = AcidCodeConstants.ADMIN_ACCOUNT_EMAIL }, AcidCodeConstants.ADMIN_ACCOUNT_PASSWORD);
            if (!result.Succeeded)
                throw new ApplicationException("Cannot create Admin user");

            var adminuser = usermanager.FindByName(AcidCodeConstants.ADMIN_ACCOUNT);
            usermanager.AddToRole(adminuser.Id, AcidCodeConstants.ADMINISTRATORS_ROLE);

            context.SaveChanges();
            base.Seed(context);
        }
    }
}
