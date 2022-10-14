using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Microsoft.AspNetCore.Mvc;
using Google_authenticate.Models;

namespace Google_authenticate.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class EventController : ControllerBase
    {
        private readonly ILogger<EventController> _logger;
        public EventController(ILogger<EventController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        async public Task<ActionResult> GetAllEventByUserId([FromQuery] string id)
        {
            if (id == null) {
                return NotFound("Wrong user id");
            } 
            try
            {
                CalendarService _service = await Common.Common.GetCalenderServiceInstance(id);

                EventsResource.ListRequest request = _service.Events.List("primary");
                // request.TimeMin = DateTime.Now;

                Events requestEvent = request.Execute();

                return Ok(requestEvent);
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        [HttpGet]
        async public Task<ActionResult> GetEventById([FromQuery] string userId, string id)
        {
            if (userId == null) {
                return NotFound("Wrong user id");
            } 
            if (id == null) {
                return NotFound("Wrong event id");
            } 

            CalendarService _service = await Common.Common.GetCalenderServiceInstance(userId);
            try
            {
                EventsResource.GetRequest request = _service.Events.Get("primary", id);
                Event getRequest = request.Execute();

                return Ok(getRequest);
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        [HttpPost]
        async public Task<ActionResult> Create([FromBody] CreateEvent createEvent)
        {
            string userId = "3";
            if (userId == null) {
                return NotFound("Wrong user id");
            } 

            CalendarService _service = await Common.Common.GetCalenderServiceInstance(userId);
            DateTime Start = Common.Common.TimeStampToDateTime(createEvent.StartTime);
            DateTime End = Common.Common.TimeStampToDateTime(createEvent.EndTime);
            IList<string> recurrence = new List<string>();
            IList<EventAttendee> attendees = new List<EventAttendee>();

            if (createEvent.AttendeesEmail != null) {
                for (int i = 0; i < createEvent.AttendeesEmail.Count; i++)
                {
                    attendees = attendees.Append(new EventAttendee{
                        Email = createEvent.AttendeesEmail[i],
                    }).ToList();
                }
            }
            
            if (createEvent.Recurrence != null) {
                for (int i = 0; i < createEvent.Recurrence.Count; i++)
                {
                    recurrence = recurrence.Append(createEvent.Recurrence[i].ToString()).ToList();
                }
            }

            Event newEvent = new Event() {
                Summary = createEvent.Summary,
                Description = createEvent.Description,
                Start = new EventDateTime()
                {
                    DateTime = Start,
                    TimeZone = "Asia/Ho_Chi_Minh",
                },
                End = new EventDateTime()
                {
                    DateTime = End,
                    TimeZone = "Asia/Ho_Chi_Minh",
                },
                Recurrence = recurrence,
                Attendees = attendees,
                // Reminders = new Event.RemindersData() {
                //     UseDefault = false,
                //     Overrides = new EventReminder[] {
                //         new EventReminder() { Method = "email", Minutes = 24 * 60 },
                //         new EventReminder() { Method = "sms", Minutes = 10 },
                //     }
                // }
            };

            try
            {
                // Load client secrets.
                string calendarId = "primary";
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
            catch (System.Exception e)
            {
                throw e;
            }
        }
    }
}