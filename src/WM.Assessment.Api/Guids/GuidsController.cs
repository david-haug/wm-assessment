using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WM.Assessment.Api.Attributes;
using WM.Assessment.Application.ExpirableGuids;
using WM.Assessment.Application.ExpirableGuids.CreateExpirableGuid;
using WM.Assessment.Application.ExpirableGuids.DeleteExpirableGuid;
using WM.Assessment.Application.ExpirableGuids.GetExpirableGuid;
using WM.Assessment.Application.ExpirableGuids.UpdateExpirableGuid;

namespace WM.Assessment.Api.Guids
{
    [Route("guid")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("CorsPolicy")]
    [HttpException]
    public class GuidController : BaseController
    {
        private readonly IMediator _mediator;

        public GuidController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        ///     Get guid by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = Constants.Routes.Guids.GetGuid)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ExpirableGuidResponse>> Get([FromRoute] string id)
        {
            return await _mediator.Send(new GetExpirableGuidRequest {Guid = id});
        }

        /// <summary>
        ///     Create guid with supplied value
        /// </summary>
        /// <remarks>Expiration time defaults to 30 days if not provided</remarks>
        /// <param name="guid"></param>
        /// <param name="request">CreateGuidModel</param>
        /// <returns></returns>
        [HttpPost("{guid}", Name = Constants.Routes.Guids.CreateGuid)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<ExpirableGuidResponse>> Post([FromRoute] string guid,
            [FromBody] CreateGuidModel request)
        {
            var result = await _mediator.Send(new CreateExpirableGuidRequest
            {
                Guid = guid,
                User = request.User,
                Expire = request.Expire
            });

            return CreatedAtRoute(Constants.Routes.Guids.CreateGuid, new {result.Guid}, result);
        }

        /// <summary>
        ///     Create auto-generated guid
        /// </summary>
        /// <remarks>Expiration time defaults to 30 days if not provided</remarks>
        /// <param name="request">CreateGuidModel</param>
        /// <returns></returns>
        [HttpPost(Name = Constants.Routes.Guids.CreateGuidWithout)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<ExpirableGuidResponse>> PostWithoutGuid([FromBody] CreateGuidModel request)
        {
            var result = await _mediator.Send(new CreateExpirableGuidRequest
            {
                User = request.User,
                Expire = request.Expire
            });

            return CreatedAtRoute(Constants.Routes.Guids.CreateGuid, new {result.Guid}, result);
        }

        /// <summary>
        ///     Update guid user and expire
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request">UpdateGuidModel</param>
        /// <returns></returns>
        [HttpPut("{guid}", Name = Constants.Routes.Guids.UpdateGuid)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ExpirableGuidResponse>> Put([FromRoute] string guid,
            [FromBody] UpdateGuidModel request)
        {
            var result = await _mediator.Send(new UpdateExpirableGuidRequest
            {
                Guid = guid,
                User = request.User,
                Expire = request.Expire
            });

            return Ok(result);
        }

        /// <summary>
        ///     Delete guid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}", Name = Constants.Routes.Guids.DeleteGuid)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Delete([FromRoute] string id)
        {
            await _mediator.Send(new DeleteExpirableGuidRequest {Guid = id});
            return NoContent();
        }
    }
}