using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MvcCookieAuthSample.Models;

namespace MvcCookieAuthSample.Data
{
    public class ApplicationDbContextSeed
    {
        UserManager<ApplicationUser> _userManager;
        RoleManager<ApplicationUserRole> _roleManager;
        public async Task SeedAsync(ApplicationDbContext context, IServiceProvider service)
        {
            if (!context.Roles.Any())
            {
                _roleManager = service.GetRequiredService<RoleManager<ApplicationUserRole>>();
                var role = new ApplicationUserRole { Name = "Administrators", NormalizedName = "Administrators" };
                var result = await _roleManager.CreateAsync(role);
                if (!result.Succeeded)
                {
                    throw new Exception("初始化角色失败");
                }
            }
            if (!context.Users.Any())
            {
                //创建一个私有域
                using (var scope = service.CreateScope())
                {
                    _userManager = service.GetRequiredService<UserManager<ApplicationUser>>();
                    

                    var user = new ApplicationUser
                    {
                        UserName = "jesse",
                        Email = "jesseTalk@163.com",
                        NormalizedUserName="jesse",
                        SecurityStamp="admin",
                        Avatar="https://chocolatey.org/content/packageimages/aspnetcore-runtimepackagestore.2.0.0.png"
                    };

                    var result= await _userManager.CreateAsync(user, "123456");
                    if (!result.Succeeded)
                    {
                        throw new Exception("初始化默认用户失败");
                    }
                    await _userManager.AddToRoleAsync(user, "Administrators");
                }
            }
        }
    }
}
