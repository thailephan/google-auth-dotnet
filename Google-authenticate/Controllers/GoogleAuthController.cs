using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Microsoft.AspNetCore.Mvc;
using Google_authenticate.Models;

namespace Google_authenticate.Controllers
{
    // Microsoft tutorial - for dotnet core 6.0 web not api
    // https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/google-logins?view=aspnetcore-6.0
    //  step 1: Install package: dotnet add package Microsoft.AspNetCore.Authentication.Google --version 6.0.10

    //  Google Authentication: https://cloud.google.com/looker/docs/admin-panel-authentication-google

    // Google dotnet API list
    // https://developers.google.com/api-client-library/dotnet
    // https://developers.google.com/api-client-library/dotnet/apis
    // https://developers.google.com/api-client-library/dotnet/apis/calendar/v3

    // Google dotnet API list - calendar
    // https://developers.google.com/calendar/api/v3/reference/events/insert#.net
    // Nuget - Google dotnet API list - calendar - install
    // https://www.nuget.org/packages/Google.Apis.Calendar.v3

    // Example: Just with console application 
    // https://github.com/googleworkspace/dotnet-samples/blob/main/calendar/CalendarQuickstart/CalendarQuickstart.cs
    // Add convert token that from user and change to access
    // https://github.com/googleapis/google-api-dotnet-client/issues/1486
    // https://stackoverflow.com/questions/38390197/how-to-create-a-instance-of-usercredential-if-i-already-have-the-value-of-access

    //  React package
    // https://www.npmjs.com/package/@react-oauth/google

    // Google user 3rd permission
    // https://myaccount.google.com/permissions

    // OAuth2 Protocol Google
    // https://developers.google.com/identity/protocols/oauth2#libraries
    [ApiController]
    [Route("[controller]")]
    public class GoogleAuthController : ControllerBase
    {
        private readonly ILogger<GoogleAuthController> _logger;

        public GoogleAuthController(ILogger<GoogleAuthController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetGoogleAuth")]
        public IEnumerable<dynamic> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new {
                Id=index,
                Token="Good"
            })
            .ToArray();
        }

        [HttpPost(Name = "PostGoogleAuth")]
        async public Task<ActionResult> AuthWithGoogle([FromBody]GoogleAuth auth) {
            if (auth.Id == null) {
                return NotFound("Thieu id user");
            }
            if (auth.Code == null) {
                return NotFound("Thieu code");
            }
            GoogleAuthorizationCodeFlow flow = Common.Common.flow;
            TokenResponse token = await flow.LoadTokenAsync(auth.Id, CancellationToken.None);
            if (token == null) {
                // token data does not exist for this user
                token = await flow.ExchangeCodeForTokenAsync(
                auth.Id, // user for tracking the userId on our backend system
                auth.Code,
                // Just be one in redirects url not need to be the same as from client's redirect uri
                "http://localhost:5173", // redirect_uri can not be empty. Must be one of the redirects url listed in your project in the api console
                CancellationToken.None);
            }

            //  Store to persistent storage
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(new FileStream("user-google.txt", FileMode.Append)))
            {
                file.WriteLine(auth.Id + "|" + token.RefreshToken + "|" + token.AccessToken + "|" + auth.Hd);
            }

            return Ok();
        }
    }
}