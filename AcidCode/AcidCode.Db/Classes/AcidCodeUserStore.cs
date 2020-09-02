using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using AcidCode.Common;

namespace AcidCode.Db
{
    public class AcidCodeUserStore : UserStore<AcidCodeUser>, IAcidCodeUserStore
    {

        public AcidCodeUserStore(IAcidCodeDbContext dbContext) : base((AcidCodeDbContext) dbContext)
        {

        }

        IEnumerable<AcidCodeUser> IAcidCodeUserStore.Users { get => Users; }
    }
}
