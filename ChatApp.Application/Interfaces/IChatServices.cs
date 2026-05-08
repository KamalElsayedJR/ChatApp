using ChatApp.Application.Dtos;
using ChatApp.Application.Dtos.Chat;
using ChatApp.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces
{
    public interface IChatServices
    {
        Task<DataResponse<ChatDto>> StartChatAsync(string ReciverId,string SendrId);
        Task<Result> SendMessageAsync(string senderId, string ChatId, string Content);
        Task<DataResponse<List<MessageDto>>> GetMessageAsync(string userId, string chatId); 
        Task<DataResponse<List<UserChatsDto>>> GetUserChatsAsync(string userId);
    }
}
