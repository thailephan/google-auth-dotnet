using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Microsoft.AspNetCore.Mvc;
using Google_authenticate.Models;
using Google_authenticate.Common;

namespace Google_authenticate.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventController : ControllerBase
    {
        private readonly ILogger<EventController> _logger;
        public EventController(ILogger<EventController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("/[controller]/GetAllEventByUserId")]
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
        [Route("/[controller]/GetEventById/{id}")]
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
    }
}