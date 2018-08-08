using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MvcCookieAuthSample.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using MvcCookieAuthSample.ViewModels;
using IdentityServer4.Services;


namespace MvcCookieAuthSample.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private IIdentityServerInteractionService _interaction;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, 
            IIdentityServerInteractionService interaction)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
        }

        //private readonly TestUserStore _users;
        //public AccountController(TestUserStore users)
        //{
        //    this._users = users;
        //}

        private IActionResult RedirectTolocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var item in result.Errors)
            {
                ModelState.AddModelError(string.Empty, item.Description);
            }
        }
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var identityUser = new ApplicationUser
                {
                    Email = registerViewModel.Email,
                    UserName = registerViewModel.Email,
                    NormalizedUserName = registerViewModel.Email
                };

                var identityResult = await _userManager.CreateAsync(identityUser, registerViewModel.Password);

                if (identityResult.Succeeded)
                {
                    await _signInManager.SignInAsync(identityUser, new AuthenticationProperties { IsPersistent = true });
                    return RedirectTolocal(returnUrl);
                }
                else
                {
                    AddErrors(identityResult);
                }
            }

            return View();
        }

        public IActionResult Login(string returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(loginViewModel.Email);
                //var user = _users.FindByUsername(loginViewModel.UserName);
                if (user == null)
                {
                    ModelState.AddModelError(nameof(LoginViewModel.Email), "userName not exists");
                }
                if (!await _userManager.CheckPasswordAsync(user, loginViewModel.Password))
                {
                    ModelState.AddModelError(nameof(LoginViewModel.Password), "password not exists");
                }
                else
                {
                    AuthenticationProperties opos = null;
                    if (loginViewModel.RememberMe)
                    {
                        opos = new AuthenticationProperties
                        {
                            IsPersistent = true,//记住我
                            ExpiresUtc = DateTime.UtcNow.Add(TimeSpan.FromMinutes(30))//过期时间,
                            
                        };
                    }

                    //await Microsoft.AspNetCore.Http.AuthenticationManagerExtensions.SignInAsync(
                    //    HttpContext,
                    //    user.SubjectId,
                    //    user.Username,
                    //    opos
                    //    );
                     await _signInManager.SignInAsync(user, opos);
                    if (_interaction.IsValidReturnUrl(returnUrl))
                    {
                       return Redirect(returnUrl);
                    }
                    return Redirect("~/");
                }
                //await _signInManager.SignInAsync(user, new AuthenticationProperties { IsPersistent = true });


            }
            return View();
        }
        public async Task<IActionResult> MarkLogin()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,"jesse"),
                new Claim(ClaimTypes.Role,"Admin")
            };
            var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimIdentity));
            return Ok();
        }

        public async Task<IActionResult> Logout()
        {
            //await HttpContext.SignOutAsync();
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}