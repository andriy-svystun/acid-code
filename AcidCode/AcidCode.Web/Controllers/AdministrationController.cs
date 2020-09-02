using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AcidCode.Common;
using AcidCode.Db;
using AcidCode.Web;
using AcidCode.Web.Models;
using Microsoft.AspNet.Identity;

namespace AcidCode.Web.Controllers
{
    [Authorize(Roles = AcidCodeConstants.ADMINISTRATORS_ROLE)]
    public class AdministrationController : Controller
    {
        private readonly IAcidCodeRepository _acidRepos;
        private readonly IAcidCodeUserStore  _userStore;

        public AdministrationController(IAcidCodeRepository acidRepos, IAcidCodeUserStore userStore)
        {
            _acidRepos = acidRepos;
            _userStore = userStore;
        }
        
        // GET: Administration
        public ActionResult Index(string section = "users")
        {
            ViewBag.CurrentPage = "Admin";

            var viewmodel = new AdministrationViewModel { Users = new List<SiteUserModel>() };

            viewmodel.SectionName = section.ToLower();

            if (section == "users")
            {
                var usermanager = new AppUserManager(_userStore);

                foreach (var user in _userStore.Users.ToList())
                {
                    SiteUserModel siteuser =AutoMapper.Mapper.Map<SiteUserModel>(user);
                    siteuser.UserRoles = usermanager.GetRoles(user.Id).ToArray();
                    viewmodel.Users.Add(siteuser);
                }
            }
            else
            {
                viewmodel.SiteSettings = _acidRepos.GetSettings();
            }
            

            return View(viewmodel);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Index(GeneralSiteSettings generalSiteSettings)
        {
            if (!ModelState.IsValid)
                return View(new AdministrationViewModel { SiteSettings = generalSiteSettings, SectionName = "settings" });

            _acidRepos.SaveSettings(generalSiteSettings);

            return View(new AdministrationViewModel { SiteSettings = generalSiteSettings, SectionName = "settings", AdminMessage = "Settings saved" });
        }

    }
}