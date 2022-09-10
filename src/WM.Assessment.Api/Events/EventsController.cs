using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WM.Assessment.Api.Attributes;
using WM.Assessment.Api.Extensions;
using WM.Assessment.Application;
using WM.Assessment.Application.EventHandling;
using WM.Assessment.Application.EventHandling.GetEvents;

namespace WM.Assessment.Api.Events
{
    [Route("events")]
    [ApiController]
    [Produces("application/json")]
    [HttpException]
    public class EventsController : BaseController
    {
        private readonly IMediator _mediator;

        public EventsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        ///     Gets all events matching supplied criteria
        /// </summary>
        /// <remarks>
        ///     Example: ../events?startDate=9/10/2022&sort=-dateOccurred&limit=10; Sort descending with "-" prefixed to field
        ///     name, separate fields with comma (sort=-dateOccurred)
        /// </remarks>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<QueryResult<ApplicationEvent>> Get([FromQuery] GetEventsModel request)
        {
            return await _mediator.Send(new GetEventsRequest
            {
                Name = request.Name,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Limit = request.Limit ?? 500,
                StartAfter = request.StartAfter,
                Sort = request.Sort.ToSortItems("DateOccurred")
            });
        }
    }
}