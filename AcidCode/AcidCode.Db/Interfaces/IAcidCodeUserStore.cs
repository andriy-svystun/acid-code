using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using AcidCode.Common;

namespace AcidCode.Db
{
    public interface IAcidCodeUserStore : IUserStore<AcidCodeUser>
    {
        IEnumerable<AcidCodeUser> Users { get; }
    }
}
