using ChatApp.Application.Dtos;
using ChatApp.Application.Dtos.Chat;
using ChatApp.Application.Interfaces;
using ChatApp.Domain.Common;
using ChatApp.Domain.Entities.ChatAggr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatServices _chatServices;

        public ChatController(IChatServices chatServices)
        {
            _chatServices = chatServices;
        }
        [HttpPost("start-chat/{reciverId}")]
        public async Task<DataResponse<ChatDto>> StartChat([FromRoute] string reciverId)
        {
            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _chatServices.StartChatAsync(reciverId, senderId);
        }
        [HttpPost("messages/{chatId}")]
        public async Task<ActionResult<Result>> SendMessage([FromRoute] string chatId, [FromBody] SendMessageDto sendMessageDto)
        {
            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _chatServices.SendMessageAsync(senderId, chatId, sendMessageDto.Content);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpGet("messages/{chatId}")]
        public async Task<ActionResult<DataResponse<List<MessageDto>>>> GetMessages([FromRoute] string chatId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _chatServices.GetMessageAsync(userId, chatId);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpGet("chats")]
        public async Task<ActionResult<DataResponse<List<UserChatsDto>>>> GetUserChats()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _chatServices.GetUserChatsAsync(userId);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
