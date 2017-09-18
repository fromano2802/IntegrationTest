using System;
using System.Threading.Tasks;
using System.Web.Http;
using IntegrationTest.Services;
using MediatR;

namespace IntegrationTest.Features
{
    [RoutePrefix("api/apikeytokens")]
    public class ApiKeyTokenController : ApiController
    {
        private readonly IMediator _mediator;
        private readonly ITrelloService _trelloService;

        public ApiKeyTokenController(IMediator mediator, ITrelloService trelloService)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _trelloService = trelloService ?? throw new ArgumentNullException(nameof(trelloService));
        }

        [HttpPost]
        [Route("apikey")]
        public async Task<IHttpActionResult> SaveApiKey([FromBody]ApiKeyTokenDto dto)
        {
            var cmd = new SaveApiKey(dto.ApiKey);
            var result = await _mediator.Send(cmd);
            return Ok(result);
        }

        [HttpDelete]
        [Route("apikey/{apiKey?}")]
        public async Task<IHttpActionResult> RemoveApiKey(string apiKey = null)
        {
            var cmd = new RemoveApiKey(apiKey ?? _trelloService.ApiKey);
            var result = await _mediator.Send(cmd);
            if (!result)
                return NotFound();
            return Ok();
        }

        [HttpPost]
        [Route("token")]
        public async Task<IHttpActionResult> SaveToken([FromBody]ApiKeyTokenDto dto)
        {
            var cmd = new SaveToken(_trelloService.ApiKey, dto.Token);
            var result = await _mediator.Send(cmd);
            return Ok(result);
        }

        [HttpDelete]
        [Route("token/{apiKey?}")]
        public async Task<IHttpActionResult> RemoveToken(string apiKey = null)
        {
            var cmd = new RemoveToken(apiKey ?? _trelloService.ApiKey);
            var result = await _mediator.Send(cmd);
            if (!result)
                return NotFound();
            return Ok();
        }
    }
}
