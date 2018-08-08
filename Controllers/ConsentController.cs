using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MvcCookieAuthSample.ViewModels;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer4.Services;
using IdentityServer4;
using MvcCookieAuthSample.Services;

namespace MvcCookieAuthSample.Controllers
{
    public class ConsentController : Controller
    {
        private readonly ConsentService _consentService;
        public ConsentController(ConsentService consentService)
        {
            _consentService = consentService;
        }
       

        
        [HttpGet]
        public async Task<IActionResult> Index(string returnUrl)
        {
            var model = await _consentService.BuildConsentViewModelAsync(returnUrl);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(InputConsentViewModel viewModel)
        {
            var result= await _consentService.ProcessConsent(viewModel);
            if (result.IsRedirect)
            { 
                return Redirect(result.RedirectUrl);
            }
            if (!string.IsNullOrEmpty(result.ValidationError))
            {
                ModelState.AddModelError("", result.ValidationError);
            }
            return View(result.ViewModel);
        }
    }
}