using AutoMapper;
using ChatApp.Application.Dtos.Chat;
using ChatApp.Application.Dtos.User;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Entities.ChatAggr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User,UserDto>().ReverseMap();
            CreateMap<Chat, ChatDto>();
            CreateMap<Message, MessageDto>();
        }
    }
}
