using Duende.IdentityServer.Models;
using Duende.IdentityServer;
using IdentityModel;

namespace CCIIdentity.Configurations
{
    public class Config
    {
        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                //Recruiter Client
                new Client
                {
                    ClientId = "recruiter",
                    ClientSecrets = { new Secret("6BF43B235893BAF9687C3F84753E3".Sha256()) },

                    AllowedGrantTypes = { GrantType.AuthorizationCode, GrantType.ResourceOwnerPassword},

                    // where to redirect to after login
                    RedirectUris = { "http://localhost:6584/signin-oidc",
                        "https://localhost:7260/signin-oidc" },
                    

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "http://localhost:6584/signout-callback-oidc",
                        "https://localhost:7260/signout-callback-oidc" },

                    AllowOfflineAccess = true,

                    AllowAccessTokensViaBrowser = true,

                    AccessTokenLifetime = 3600, //1 hours

                    AbsoluteRefreshTokenLifetime = 2592000, //30 days

                    SlidingRefreshTokenLifetime = 1296000, //15 days

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        IdentityServerConstants.StandardScopes.OpenId,
                        "recruiter"
                    }
                },

                //Admin Client
                new Client
                {
                    ClientId = "admin",
                    ClientSecrets = { new Secret("Enp23ZUd80hLjxlkeFZo1sdX3wAHJtNw".Sha256()) },

                    AllowedGrantTypes = { GrantType.AuthorizationCode, GrantType.ResourceOwnerPassword},

                    // where to redirect to after login
                    RedirectUris = { "http://localhost:6584/signin-oidc",
                        "https://localhost:7260/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "http://localhost:6584/signout-callback-oidc",
                        "https://localhost:7260/signout-callback-oidc" },

                    AllowOfflineAccess = true,

                    AllowAccessTokensViaBrowser = true,

                    AccessTokenLifetime = 3600, //1 hours

                    AbsoluteRefreshTokenLifetime = 2592000, //30 days

                    SlidingRefreshTokenLifetime = 1296000, //15 days

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        IdentityServerConstants.StandardScopes.OpenId,
                        "admin"
                    }
                },

                //User Client
                new Client
                {
                    ClientId = "user",
                    ClientSecrets = { new Secret("dmAfzutnfoupYgRxs1fOk6IoTCrYL3Io".Sha256()) },

                    AllowedGrantTypes = { GrantType.AuthorizationCode, GrantType.ResourceOwnerPassword},

                    // where to redirect to after login
                    RedirectUris = { "http://localhost:6584/signin-oidc",
                        "https://localhost:7260/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "http://localhost:6584/signout-callback-oidc",
                        "https://localhost:7260/signout-callback-oidc" },

                    AllowOfflineAccess = true,

                    AllowAccessTokensViaBrowser = true,

                    AccessTokenLifetime = 3600, //1 hours

                    AbsoluteRefreshTokenLifetime = 2592000, //30 days

                    SlidingRefreshTokenLifetime = 1296000, //15 days

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        IdentityServerConstants.StandardScopes.OpenId,
                        "admin"
                    }
                },
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope(name: "recruiter", displayName: "Recruiter API"),
                new ApiScope(name: "admin", displayName: "Admin API"),
                new ApiScope(name: "user", displayName: "User API")
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new List<ApiResource>
            {
            };

        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResource(
                    name: "UserProfile",
                    userClaims: new[] { JwtClaimTypes.Id, JwtClaimTypes.Name, JwtClaimTypes.Email, JwtClaimTypes.PhoneNumber, JwtClaimTypes.Role },
                    displayName: "User profile data"),

                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
    }
}
