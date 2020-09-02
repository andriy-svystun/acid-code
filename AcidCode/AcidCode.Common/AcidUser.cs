using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AcidCode.Common
{
    public class AcidCodeUser : IdentityUser
    {
        public ICollection<CodeItem> CodeItem { get; set; }
    }
}
