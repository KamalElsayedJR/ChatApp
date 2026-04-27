using ChatApp.Application.Dtos;
using ChatApp.Application.Dtos.User;
using ChatApp.Domain.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces
{
    public interface IUserServices
    {
        public Task<DataResponse<UserDto>> UpdateFullName(string userId, string fullName);
        public Task<DataResponse<UserDto>> UpdateProfilePicture(string userId, IFormFile file);
        public Task<DataResponse<UserDto>> UpdateBio(string userId, string bio);
        public Task<DataResponse<UserDto>> UpdateEmail(string userId, string email);
        //public Task<DataResponse<UserDto>> ChangePassword(string userId, string currentPassword, string newPassword);
        public Task<DataResponse<SearchedUsers>> GetUserById(string userId);
        public Task<DataResponse<List<SearchedUsers>>> SearchUsers(string query);
    }
}
