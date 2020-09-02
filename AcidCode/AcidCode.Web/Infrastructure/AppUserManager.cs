using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using AcidCode.Common;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.AspNet.Identity.Owin;
using AcidCode.Db;

namespace AcidCode.Web
{
    public class AppUserManager : UserManager<AcidCodeUser>
    {
        public AppUserManager(IUserStore<AcidCodeUser> store)
                : base(store)
        {
        }

        // this method is called by Owin therefore best place to configure your User Manager
        public static AppUserManager Create(
            IdentityFactoryOptions<AppUserManager> options, IOwinContext context)
        {
            var manager = new AppUserManager(
                new UserStore<AcidCodeUser>(context.Get<AcidCodeDbContext>()));

            // optionally configure your manager
            // ...

            return manager;
        }
    }
}
