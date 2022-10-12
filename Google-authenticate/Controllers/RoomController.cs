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
    [ApiController]
    [Route("[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly ILogger<GoogleAuthController> _logger;
        public RoomController(ILogger<GoogleAuthController> logger)
        {
            _logger = logger;

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
        }

        GoogleAuthorizationCodeFlow _flow;
        string[] Scopes = {
            CalendarService.Scope.CalendarEvents,
            CalendarService.Scope.Calendar,
        };
        const string credPath = "token.json";

        [HttpGet(Name = "GetRooms")]
        async public Task<ActionResult> Get([FromQuery] string id)
        {
            FileStream fileStream = new FileStream("user-google.txt", FileMode.Open);
            string? userId = null;
            string? refreshToken = null, accessToken = null, hd = null;
            using (StreamReader reader = new StreamReader(fileStream))
            {
                string? line = reader.ReadLine();
                while (line != null) {
                    string[] items = line.Split("|");
                    string _userId = items[0];

                    if (_userId == id) {
                        userId = _userId;
                        refreshToken = items[1];
                        accessToken = items[2];
                        hd = items[3];
                    }
                    line = reader.ReadLine();
                }
            }

            if (userId == null) {
                return NotFound("Wrong user id");
            } 
            // TokenResponse token = await _flow.RefreshTokenAsync(userId, refreshToken, CancellationToken.None);
            TokenResponse token = await _flow.LoadTokenAsync(userId, CancellationToken.None);

            UserCredential _credential = new UserCredential(_flow, userId, token); 

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
                    DateTime = DateTime.Parse("2022-12-10T14:00:00+07:00"),
                    TimeZone = "Asia/Ho_Chi_Minh",
                },
                End = new EventDateTime()
                {
                    DateTime = DateTime.Parse("2022-12-10T16:00:00+07:00"),
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

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(new FileStream("rooms.txt", FileMode.Append)))
            {
                file.WriteLine(id + "|" + patchedEvent.HangoutLink);
            }

            return Ok(new {
                token = token,
                hd = hd,
                meet = patchedEvent.HangoutLink,
            });
        }

        [HttpPost(Name = "CreateRooms")]
        async public Task<ActionResult> Create([FromQuery] string id)
        {
            // Get user refreshToken
            FileStream fileStream = new FileStream("user-google.txt", FileMode.Open);
            string? userId = null;
            string? refreshToken = null, accessToken = null;
            using (StreamReader reader = new StreamReader(fileStream))
            {
                string? line = reader.ReadLine();
                while (line != null) {
                    string[] items = line.Split("|");
                    string _userId = items[0];

                    if (_userId == id) {
                        userId = _userId;
                        refreshToken = items[1];
                        accessToken = items[2];
                    }
                    line = reader.ReadLine();
                }
            }
            //  Verify found user
            if (userId == null) {
                return NotFound("Wrong user id");
            } 

            // Load new Token Async base on userId
            TokenResponse token = await _flow.LoadTokenAsync(userId, CancellationToken.None);

            UserCredential _credential = new UserCredential(_flow, userId, token); 

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
                    DateTime = DateTime.Parse("2022-12-10T14:00:00+07:00"),
                    TimeZone = "Asia/Ho_Chi_Minh",
                },
                End = new EventDateTime()
                {
                    DateTime = DateTime.Parse("2022-12-10T16:00:00+07:00"),
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
            return Ok(token);
        }
    }
}