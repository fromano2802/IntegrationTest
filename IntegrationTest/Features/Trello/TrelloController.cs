using System;
using System.Threading.Tasks;
using System.Web.Http;
using IntegrationTest.Infrastructure;
using IntegrationTest.Services;

namespace IntegrationTest.Features.Trello
{
    [WebApiAuthorization]
    [RoutePrefix("api/trello")]
    public class TrelloController : ApiController
    {
        private readonly ITrelloService _trelloService;

        public TrelloController(ITrelloService trelloService)
        {
            _trelloService = trelloService ?? throw new ArgumentNullException(nameof(trelloService));
        }

        [HttpGet]
        [Route("token")]
        public async Task<IHttpActionResult> GetToken()
        {
            var result = await _trelloService.GetTokenAsync();
            return Ok(result);
        }

        [HttpGet]
        [Route("members/{memberId}")]
        public async Task<IHttpActionResult> GetMember(string memberId)
        {
            var result = await _trelloService.GetMemberAsync(memberId != "me" ? memberId : null);
            return Ok(result);
        }

        [HttpGet]
        [Route("members/{memberId}/boards")]
        public async Task<IHttpActionResult> GetBoards(string memberId)
        {
            var result = await _trelloService.GetBoardsAsync(memberId != "me" ? memberId : null);
            return Ok(result);
        }

        [HttpGet]
        [Route("boards/{boardId}")]
        public async Task<IHttpActionResult> GetBoard(string boardId)
        {
            var result = await _trelloService.GetBoardAsync(boardId);
            return Ok(result);
        }

        [HttpGet]
        [Route("boards/{boardId}/cards")]
        public async Task<IHttpActionResult> GetCards(string boardId)
        {
            var result = await _trelloService.GetCardsAsync(boardId);
            return Ok(result);
        }

        [HttpGet]
        [Route("cards/{cardId}")]
        public async Task<IHttpActionResult> GetCard(string cardId)
        {
            var result = await _trelloService.GetCardAsync(cardId);
            return Ok(result);
        }

        [HttpGet]
        [Route("cards/{cardId}/comments")]
        public async Task<IHttpActionResult> GetComments(string cardId)
        {
            var result = await _trelloService.GetCommentCardActionsAsync(cardId);
            return Ok(result);
        }

        [HttpGet]
        [Route("comments/{commentId}")]
        public async Task<IHttpActionResult> GetComment(string commentId)
        {
            var result = await _trelloService.GetCommentCardActionAsync(commentId);
            return Ok(result);
        }

        [HttpPost]
        [Route("cards/{cardId}/comments")]
        public async Task<IHttpActionResult> PostComment(string cardId, [FromBody]CommentDto dto)
        {
            var result = await _trelloService.PostCommentCardActionAsync(cardId, dto.Comment);
            return Ok(result);
        }
    }
}
