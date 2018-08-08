using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using MvcCookieAuthSample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MvcCookieAuthSample.Services
{
    public class ProfileService : IProfileService
    {
        private UserManager<ApplicationUser> _userManager;

        public ProfileService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;

        }
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subjectId = context.Subject.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            var user = await _userManager.FindByIdAsync(subjectId);
            var claims = await GetClaimsByUserAsync(user);
            context.IssuedClaims = claims;
        }

        private async Task<List<Claim>> GetClaimsByUserAsync(ApplicationUser user)
        {
            var claims = new List<Claim> {
                new Claim(JwtClaimTypes.Subject,user.Id.ToString()),
                new Claim(JwtClaimTypes.PreferredUserName,user.UserName)
                
            };

            var rols = await _userManager.GetRolesAsync(user);
            foreach (var role in rols)
            {
                claims.Add(new Claim(JwtClaimTypes.Role, role));
            }

            if (!string.IsNullOrWhiteSpace(user.Avatar))
            {
                claims.Add(new Claim("avatr", user.Avatar));
            }
            return claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = false;
            var subjectId= context.Subject.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            var user = await _userManager.FindByIdAsync(subjectId);

            context.IsActive = user != null;

        }
    }
}
