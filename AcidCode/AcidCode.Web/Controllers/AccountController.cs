using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AcidCode.Common;
using AcidCode.Db;
using AcidCode.Web.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;

namespace AcidCode.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IAcidCodeDbContext _dbcontext;
        private readonly IAcidCodeUserStore _userStore;

        public AccountController(IAcidCodeDbContext dbContext, IAcidCodeUserStore userStore)
        {
            _dbcontext = dbContext;
            _userStore = userStore;
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return new HttpUnauthorizedResult();
            }

            ViewBag.returnUrl = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var usermanager = new AppUserManager(_userStore);

            var signinmanager = new SignInManager<AcidCodeUser, string>(usermanager, AuthManager);

            var result = await signinmanager.PasswordSignInAsync(model.Name, model.Password, false, shouldLockout: false);

            switch (result)
            {
                case SignInStatus.Success:
                    {
                        return RedirectToLocal(returnUrl);
                    }

                case SignInStatus.LockedOut:
                    {
                        ViewBag.returnUrl = returnUrl;
                        ModelState.AddModelError("", "Your account is locked");
                        return View(model);
                    }
                case SignInStatus.RequiresVerification:
                    return new HttpUnauthorizedResult(); 
                case SignInStatus.Failure:
                default:
                    {
                        ViewBag.returnUrl = returnUrl;
                        ModelState.AddModelError("", "Invalid login attempt.");
                        return View(model);
                    }
            }
        }

        [AllowAnonymous]
        public ActionResult Register(string returnUrl)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return new HttpUnauthorizedResult();
            }

            ViewBag.returnUrl = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.IsSucceeded = false;
                return View(model);
            }

            var newuser = new AcidCodeUser
            {
                UserName = model.UserName,
            };

            var usermanager = new AppUserManager(_userStore);

            var result = await usermanager.CreateAsync(newuser, model.Password);

            ViewBag.returnUrl = model.ReturnUrl;
            if (!result.Succeeded)
            {
                model.IsSucceeded = false;
                foreach(var item in result.Errors)
                {
                    ModelState.AddModelError("",item);
                }
                return View("RegisterResult", model);
            }

            model.IsSucceeded = true;
            return View("RegisterResult", model);
        }

        public ActionResult Logout(string returnUrl)
        {
            AuthManager.SignOut();

            return Redirect(returnUrl);
        }

        private IAuthenticationManager AuthManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }


    }
}