using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using Google.Apis.Calendar.v3;

namespace Google_authenticate.Common {
    public class Common {
        static GoogleAuthorizationCodeFlow flow = Common.GetAuthorizationCodeFlow();
        public static GoogleAuthorizationCodeFlow GetAuthorizationCodeFlow() {
            GoogleAuthorizationCodeFlow _flow;
            string[] Scopes = {
                CalendarService.Scope.CalendarEvents,
                CalendarService.Scope.Calendar,
            };
            const string credPath = "token.json";

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
                });
                return _flow;
            }
        }
        public async static Task<CalendarService> GetCalenderServiceInstance(string userId) {
            TokenResponse token = await flow.LoadTokenAsync(userId, CancellationToken.None);
            UserCredential _credential = new UserCredential(flow, userId, token); 
            // Load client secrets.
            CalendarService _service = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = _credential,
            });

            return _service;
        }
    }
}