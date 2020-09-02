using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AcidCode.Web.Models
{
    public class SiteUserModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string[] UserRoles { get; set; } 
        public string Email { get; set; }
    }
}