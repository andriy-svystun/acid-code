using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AcidCode.Common;

namespace AcidCode.Web.Models
{
    public class AdministrationViewModel
    {
        public ICollection<SiteUserModel> Users { get; set; }
        public GeneralSiteSettings SiteSettings { get; set; }

        public string SectionName { get; set; }
        public string AdminMessage { get; set; }
    }
}