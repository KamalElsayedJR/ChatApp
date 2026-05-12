using AutoMapper;
using ChatApp.Application.Dtos;
using ChatApp.Application.Dtos.Chat;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Repositories;
using ChatApp.Application.Specification.ChatSpec;
using ChatApp.Application.Specification.UserSpec;
using ChatApp.Domain.Common;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Entities.ChatAggr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Services
{
    public class ChatServices : IChatServices
    {
        private readonly IUnitOfWork _uoW;
        private readonly IMapper _mapper;

        public ChatServices(IUnitOfWork UoW, IMapper mapper)
        {
            _uoW = UoW;
            this._mapper = mapper;
        }

        public async Task<DataResponse<List<MessageDto>>> GetMessageAsync(string userId, string chatId, int pageIndex = 1, int pageSize = 20)
        {
            var spec = new ChatWithParticipantsSpec(chatId);
            var chat = await _uoW.Repository<Chat>().GetOneWithSpecAsync(spec);
            if (chat == null || !chat.ChatParticipants.Any(c => c.UserId == userId))
            {
                return DataResponse<List<MessageDto>>.Failure("Chat not found");
            }

            var paginatedMessages = chat.Messages
                .OrderBy(m => m.SentAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return DataResponse<List<MessageDto>>.Success(_mapper.Map<List<MessageDto>>(paginatedMessages), "Chat retrieved successfully");
        }
        public async Task<DataResponse<List<UserChatsDto>>> GetUserChatsAsync(string userId)
        {
            var spec = new ChatsForUserSpec(userId);
            var chats = await _uoW.Repository<Chat>().GetAllWithSpec(spec);
            if (chats is null)
            {
                return DataResponse<List<UserChatsDto>>.Failure("No chats found");
            }
            var result = chats.Select(c =>
            {
                var otherUser = c.ChatParticipants
                    .FirstOrDefault(cp => cp.UserId != userId)?.User;
                var lastMessage = c.Messages
                                .OrderByDescending(m => m.SentAt)
                                .FirstOrDefault();
                return new UserChatsDto
                {
                    ChatId = c.Id,
                    FullName = otherUser?.FullName ?? "",
                    UserName = otherUser?.UserName ?? "",
                    ProfilePictureURL = otherUser?.ProfilePictureURL ?? "",
                    LastMessage = lastMessage?.Content,
                    LastMessageAt = lastMessage?.SentAt,
                    LastMessageTime = lastMessage?.SentAt,
                    LastMessageSenderId = lastMessage?.SenderId,
                    UnreadCount = c.Messages.Count(m => !m.IsRead && m.SenderId != userId)
                };
            }).ToList();

            return DataResponse<List<UserChatsDto>>.Success(result, "Chats retrieved successfully");
        }
        public async Task<Result> SendMessageAsync(string senderId, string ChatId, string Content)
        {
            if (string.IsNullOrWhiteSpace(Content))
            {
                return Result.Failure("Message content cannot be empty");
            }
            var chatspec = new ChatWithParticipantsSpec(ChatId);
            var chat = await _uoW.Repository<Chat>().GetOneWithSpecAsync(chatspec);
            if (chat == null || !chat.ChatParticipants.Any(c => c.UserId == senderId))
            {
                return Result.Failure("Chat not found");
            }
            var message = new Message
            {
                ChatId = ChatId,
                SenderId = senderId,
                Content = Content,
            };
            chat.Messages.Add(message);
            var result = await _uoW.SaveChangesAsync();
            if (result <= 0)
            {
                return Result.Failure("Failed to send message");
            }
            return Result.Success("Message sent successfully");
        }
        public async Task<DataResponse<ChatDto>> StartChatAsync(string ReciverId, string SendrId)
        {
            var sender = await _uoW.Repository<User>().GetByIdAsync(SendrId);
            if (sender == null)
            {
                return DataResponse<ChatDto>.Failure("Sender not found");
            }
            var reciver = await _uoW.Repository<User>().GetByIdAsync(ReciverId);
            if (reciver == null)
            {
                return DataResponse<ChatDto>.Failure("Reciver not found");
            }
            if (SendrId == ReciverId)
            {
                return DataResponse<ChatDto>
                    .Failure("You can't start chat with yourself");
            }
            var spec = new ChatBetweenUsersSpec(SendrId, ReciverId);
            var existingChat = await _uoW.Repository<Chat>().GetOneWithSpecAsync(spec);
            if (existingChat is not null)
            {
                return DataResponse<ChatDto>.Success(_mapper.Map<ChatDto>(existingChat), "Chat already exists");
            }
            var chat = new Chat
            {
                ChatParticipants = new List<ChatParticipant>
                {
                    new ChatParticipant { UserId = SendrId },
                    new ChatParticipant { UserId = ReciverId }
                }
            };
            await _uoW.Repository<Chat>().AddAsync(chat);
            var result = await _uoW.SaveChangesAsync();
            if (result <= 0)

            {
                return DataResponse<ChatDto>.Failure("Failed to save chat");
            }
            return DataResponse<ChatDto>.Success(_mapper.Map<ChatDto>(chat), "Chat started successfully");
        }

        public async Task<Result> EditMessageAsync(string userId, string messageId, string newContent)
        {
            if (string.IsNullOrWhiteSpace(newContent))
                return Result.Failure("Content cannot be empty");

            var message = await _uoW.Repository<Message>().GetByIdAsync(messageId);
            if (message == null) return Result.Failure("Message not found");
            if (message.SenderId != userId) return Result.Failure("Unauthorized");
            if (message.IsDeleted) return Result.Failure("Cannot edit deleted message");

            message.Content = newContent;
            message.IsEdited = true;
            message.EditedAt = DateTime.UtcNow;

            await _uoW.SaveChangesAsync();

            return Result.Success("Message edited successfully");
        }

        public async Task<Result> DeleteMessageAsync(string userId, string messageId)
        {
            var message = await _uoW.Repository<Message>().GetByIdAsync(messageId);
            if (message == null) return Result.Failure("Message not found");
            if (message.SenderId != userId) return Result.Failure("Unauthorized");

            message.IsDeleted = true;
            message.DeletedAt = DateTime.UtcNow;
            message.Content = "This message was deleted";

            await _uoW.SaveChangesAsync();

            return Result.Success("Message deleted successfully");
        }

        public async Task<Result> MarkMessageAsSeenAsync(string userId, string messageId)
        {
            var message = await _uoW.Repository<Message>().GetByIdAsync(messageId);
            if (message == null) return Result.Failure("Message not found");

            var chat = await _uoW.Repository<Chat>().GetByIdAsync(message.ChatId);
            // Wait, we need to know if user is part of the chat and not the sender.
            if (message.SenderId == userId) return Result.Failure("Sender cannot mark own message as seen");

            message.MessageStatus = Domain.Enums.MessageStatus.Seen;
            message.IsRead = true;
            message.ReadAt = DateTime.UtcNow;

            await _uoW.SaveChangesAsync();

            return Result.Success("Message marked as seen");
        }
    }
}
