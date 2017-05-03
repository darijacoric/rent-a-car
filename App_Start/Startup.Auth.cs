using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using RentACar.Models;
using RentACar.DAT;
using Microsoft.Owin.Security.Facebook;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using RentACar.App_Helpers;

namespace RentACar
{
    public enum AuthenticationType { Facebook, Google };

    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(UserDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, AppUser, int>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentityCallback: (manager, user) => user.GenerateUserIdentityAsync(manager),
                        getUserIdCallback: (id) => (id.GetUserId<int>()))
                }
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);
            
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");


            var facebookAuthenticationOptions = new FacebookAuthenticationOptions()
            {                                
                // App ID that Facebook provides for using external Faceboook authentication
                AppId = "",
                // App Secret that Facebook provides for using external Faceboook authentication
                AppSecret = "",
                AuthenticationType = "Facebook",
                SignInAsAuthenticationType = DefaultAuthenticationTypes.ExternalCookie,
                Provider = new FacebookAuthenticationProvider()
                {
                    OnAuthenticated = async (context) =>
                    context.Identity.AddClaims(new List<Claim>()
                        {
                            new Claim("FacebookAccessToken", context.AccessToken)
                        })
                }
            };

            facebookAuthenticationOptions.Scope.Add("email");
            facebookAuthenticationOptions.Scope.Add("public_profile");           

            app.UseFacebookAuthentication(facebookAuthenticationOptions);

            var googleOptions = new GoogleOAuth2AuthenticationOptions()
            {
                // Client ID that Google provides for using GoogleOAuth2
                ClientId = "",
                // Client Secret that Google provides for using GoogleOAuth2
                ClientSecret = "",
                AuthenticationType = "Google",
                SignInAsAuthenticationType = DefaultAuthenticationTypes.ExternalCookie,                
                Provider = new GoogleOAuth2AuthenticationProvider()
                {
                    OnAuthenticated = async (context) =>
                    {
                        context.Identity.AddClaim(new Claim(AppValues.GoogleOAuth2.NAME, context.Identity.FindFirstValue(ClaimTypes.Name)));
                        context.Identity.AddClaim(new Claim(AppValues.GoogleOAuth2.GIVENNAME, context.Identity.FindFirstValue(ClaimTypes.GivenName)));
                        context.Identity.AddClaim(new Claim(AppValues.GoogleOAuth2.SURNAME, context.Identity.FindFirstValue(ClaimTypes.Surname)));
                        //context.Identity.AddClaim(new Claim(AppValues.GoogleOAuth2.BIRTHDATE, context.Identity.FindFirstValue(ClaimTypes.DateOfBirth))); // ExternalLoginInfo loginfo in AccountController is null if this claim is set
                        context.Identity.AddClaim(new Claim(AppValues.GoogleOAuth2.EMAIL, context.Identity.FindFirstValue(ClaimTypes.Email)));
                        context.Identity.AddClaim(new Claim(AppValues.GoogleOAuth2.ACCESSTOKEN, context.AccessToken, ClaimValueTypes.String, "Google"));
                    }
                }
            };
            
            app.UseGoogleAuthentication(googleOptions);
         
            //#endregion
        }
    }
}