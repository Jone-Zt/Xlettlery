using Microsoft.Owin.Security.OAuth;
using ServicesInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Tools;

namespace MobileAPI.OAuth
{
    public class SimpleAuthorizationServerProvider: OAuthAuthorizationServerProvider
    {
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.FromResult<object>(null);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            IAuthorizeInterface proxy = RemotingAngency.GetRemoting().GetProxy<IAuthorizeInterface>();
            if (proxy == null) {
                    context.SetError("invalid_grant", "privilege grant failed!");
                    return;
            }
            if (proxy.CheckAuthorzeServices(context.UserName, context.Password) == false) {
                   context.SetError("invalid_grant", "The username or password is incorrect");
                    return;
            }
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim("sub", context.UserName));
            identity.AddClaim(new Claim("role", "user"));
            context.Validated(identity);

        }
    }
}