using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Test;
using IdentityServer4;
using System.Security.Claims;

namespace MvcCookieAuthSample
{
    public class Config
    {
        public static IEnumerable<ApiResource> GetResources()
        {
            return new List<ApiResource> {
                new ApiResource("api","identityserver api")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client> {
                new Client{
                    ClientId="mvc",
                    ClientName="mvc client",
                    ClientUri="http://localhost:5001",
                    LogoUri="/images/timg.jpg",
                    AllowRememberConsent=true,

                    AllowedGrantTypes=GrantTypes.Implicit,
                    ClientSecrets={
                        new Secret("secret".Sha256())
                    },
                    RequireConsent=true,
                    RedirectUris={"http://localhost:5001/signin-oidc"},
                    PostLogoutRedirectUris={ "http://localhost:5001/signout-callback-oidc"},
                    AlwaysIncludeUserClaimsInIdToken =true,
                    AllowedScopes={
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Email
                    }
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource> {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
            };
        }

        public static List<TestUser> GetTestUsers()
        {
            return new List<TestUser> {
                new TestUser{
                    SubjectId="10000",
                    Username="jesse",
                    Password="123456",
                    Claims=new List<Claim>{
                        new Claim("name","jesse"),
                        new Claim("website","video.jessetalk.cn")
                    }
                },                
            };
        }
    }
}
