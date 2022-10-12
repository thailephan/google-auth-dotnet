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
        GoogleAuthorizationCodeFlow _flow;
        string[] Scopes = {
            CalendarService.Scope.CalendarEvents,
            CalendarService.Scope.Calendar,
        };
        // const string userId = "1";
        // const string authorizationCode = "4/0ARtbsJpJt11ziXd_nSQl8GskSVfJV5iRFLEVcSkw1vVic39OhgILPpDkWy3GNikdN6pBVw";
        const string credPath = "token.json";

        public GoogleAuthController(ILogger<GoogleAuthController> logger)
        {
            _logger = logger;

            //* Way 1 *//
            // Load client secrets.
            using (var stream =
                   new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                /* The file token.json stores the user's access and refresh tokens, and is created
                 automatically when the authorization flow completes for the first time. */
                _flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes = Scopes,
                    DataStore = new FileDataStore(credPath, true)
                //  DataStore = new FileDataStore("Store")
                });
            }

            //* End way 1 *//

            //* Way 2 *//
            // Token not auto refresh
            // GoogleCredential cred = GoogleCredential.FromAccessToken("ya29.a0Aa4xrXMwYuGv6ZsWBLNum1E6abGLuPzqlI2de5oJg1Kwj77BaWyjZR-wdB1KnqWuAG1YMcImAbINtKM31KfQmK00XwYdKwcdT8-dx77dOMlSoEnqrbaajePC_T-LAWvn8f-KirGjIsP4O3bPKGrXQLI2Pp8BUP6j_a8oLAaCgYKATASAQ8SFQEjDvL9qOL0w_hpvpQGJFnx3YcIEQ0173");
            // _service = new CalendarService(new BaseClientService.Initializer {HttpClientInitializer = cred});
            //* End way 2 *//
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
        async public Task<ObjectResult> AuthWithGoogle([FromBody]GoogleAuth auth) {
            // Refer to the .NET quickstart on how to setup the environment:
            // https://developers.google.com/calendar/quickstart/dotnet
            // Change the scope to CalendarService.Scope.Calendar and delete any stored
            // credentials.
            if (auth.Code == null) {
                return NotFound("Thieu code");
            }
            UserCredential _credential;

            FileDataStore fileStore = new FileDataStore(credPath, true);
            TokenResponse token = await fileStore.GetAsync<TokenResponse>(auth.Id);

            if (token == null) {
                // token data does not exist for this user
                token = await _flow.ExchangeCodeForTokenAsync(
                auth.Id, // user for tracking the userId on our backend system
                auth.Code,
                // Just be one in redirects url not need to be the same as from client's redirect uri
                "http://localhost:5173", // redirect_uri can not be empty. Must be one of the redirects url listed in your project in the api console
                CancellationToken.None);
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(new FileStream("user-google.txt", FileMode.Append)))
            {
                file.WriteLine(auth.Id + "|" + token.RefreshToken + "|" + token.AccessToken + "|" + auth.Hd);
            }

            _credential = new UserCredential(_flow, auth.Id, token); 

            // Load client secrets.
            CalendarService _service = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = _credential,
            });
            Event newEvent = new Event() {
                Summary = "11 Maysoft festival day",
                Description = "11 Maysoft festival day",
                Start = new EventDateTime()
                {
                    DateTime = DateTime.Parse("2022-12-08T14:00:00+07:00"),
                    TimeZone = "Asia/Ho_Chi_Minh",
                },
                End = new EventDateTime()
                {
                    DateTime = DateTime.Parse("2022-12-08T16:00:00+07:00"),
                    TimeZone = "Asia/Ho_Chi_Minh",
                },
                Recurrence = new String[] { "RRULE:FREQ=WEEKLY;UNTIL=20221231T000000Z;BYDAY=TU,TH,SA" },
                Attendees = new EventAttendee[] {
                    new EventAttendee() { Email = "jpett612@gmail.com" },
                },
                // Reminders = new Event.RemindersData() {
                //     UseDefault = false,
                //     Overrides = new EventReminder[] {
                //         new EventReminder() { Method = "email", Minutes = 24 * 60 },
                //         new EventReminder() { Method = "sms", Minutes = 10 },
                //     }
                // }
            };

            String calendarId = "primary";
            EventsResource.InsertRequest insertRequest = _service.Events.Insert(newEvent, calendarId);
            Event createdEvent = insertRequest.Execute();

            EventsResource.PatchRequest patchRequest = _service.Events.Patch(new Event {
                ConferenceData = new ConferenceData {
                    CreateRequest = new CreateConferenceRequest {
                        RequestId = "a",
                    },
                },
            }, calendarId, createdEvent.Id);
            patchRequest.SendUpdates = EventsResource.PatchRequest.SendUpdatesEnum.All;
            patchRequest.ConferenceDataVersion = 1;

            Event patchedEvent = patchRequest.Execute();

            return Ok(patchedEvent);
        }
    }
}