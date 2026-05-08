using AutoMapper;
using ChatApp.Application.Dtos;
using ChatApp.Application.Dtos.User;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Repositories;
using ChatApp.Application.Specification.UserSpec;
using ChatApp.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Services
{
    public class UserServices : IUserServices
    {
        private readonly IUnitOfWork _uoW;
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;

        public UserServices(IUnitOfWork UoW,IImageService imageService,IMapper mapper)
        {
            _uoW = UoW;
            this._imageService = imageService;
            this._mapper = mapper;
        }

        public async Task<DataResponse<SearchedUsers>> GetUserById(string userId)
        {
            var user = await _uoW.Repository<User>().GetByIdAsync(userId);
            if (user == null)
            {
                return DataResponse<SearchedUsers>.Failure("User not found");
            }
            var userDto = new SearchedUsers
            {
                FullName = user.FullName,
                Email = user.Email,
                UserName = user.UserName,
                ProfilePictureURL = user.ProfilePictureURL,
                Bio = user.Bio,
            };
            return DataResponse<SearchedUsers>.Success(userDto);
        }
        public async Task<DataResponse<List<SearchedUsers>>> SearchUsers(string query)
        {
            var spec = new AllUsersWithCondition(query);
            var users = await _uoW.Repository<User>().GetAllWithSpec(spec);
            var userDtos = users.Select(user => new SearchedUsers
            {
                FullName = user.FullName,
                Email = user.Email,
                UserName = user.UserName,
                ProfilePictureURL = user.ProfilePictureURL,
                Bio = user.Bio,
            }).ToList();
            if (userDtos.Count == 0)
            {
                return DataResponse<List<SearchedUsers>>.Failure("No users found matching the search criteria");
            }
            return DataResponse<List<SearchedUsers>>.Success(userDtos, "Users found");
        }
        public async Task<DataResponse<UserDto>> UpdateBio(string userId, string bio)
        {
            var user = await _uoW.Repository<User>().GetByIdAsync(userId);
            if (user == null)
            {
                return DataResponse<UserDto>.Failure("User not found");
            }
            user.UpdateBio(bio);
            var result = await _uoW.SaveChangesAsync();
            if (result <= 0)
            {
                return DataResponse<UserDto>.Failure("Failed to update profile");
            }
            
            var userDto = _mapper.Map<User, UserDto>(user);

            return DataResponse<UserDto>.Success(userDto);
        }

        public async Task<DataResponse<UserDto>> UpdateEmail(string userId, string email)
        {
            var user = await _uoW.Repository<User>().GetByIdAsync(userId);
            if (user == null)
            {
                return DataResponse<UserDto>.Failure("User not found");
            }
            if (string.IsNullOrEmpty(email))
            {
                return DataResponse<UserDto>.Failure("Email cannot be empty");
            }
            user.ChangeEmail(email);
            var result = await _uoW.SaveChangesAsync();
            if (result <= 0)
            {
                return DataResponse<UserDto>.Failure("Failed to update profile");
            }
            var userDto = _mapper.Map<User, UserDto>(user);

            return DataResponse<UserDto>.Success(userDto);
        }

        public async Task<DataResponse<UserDto>> UpdateFullName(string userId, string fullName)
        {
            var user = await _uoW.Repository<User>().GetByIdAsync(userId);
            if (user == null)
            {
                return DataResponse<UserDto>.Failure("User not found");
            }
            if (string.IsNullOrEmpty(fullName))
            {
                return DataResponse<UserDto>.Failure("Full name cannot be empty");
            }
            user.UpdateFullName(fullName);
            var result = await _uoW.SaveChangesAsync();
            if (result <= 0)
            {
                return DataResponse<UserDto>.Failure("Failed to update profile");
            }
            
            var userDto = _mapper.Map<User, UserDto>(user);

            return DataResponse<UserDto>.Success(userDto);
        }

        public async Task<DataResponse<UserDto>> UpdateProfilePicture(string userId, IFormFile File)
        {
            if (File == null || File.Length == 0)
            {
                return DataResponse<UserDto>.Failure("No file uploaded");
            }
            if (!File.ContentType.StartsWith("image/"))
            {
                return DataResponse<UserDto>.Failure("Invalid file type. Only images are allowed");
            }
            if (File.Length > 5 * 1024 * 1024)
            {
                return DataResponse<UserDto>.Failure("File size exceeds the limit of 5MB");
            }
            var imageurl = await _imageService.UploadImageAsync(File);
            if (string.IsNullOrEmpty(imageurl))
            {
                return DataResponse<UserDto>.Failure("Failed to upload image");
            }
            var user = await _uoW.Repository<User>().GetByIdAsync(userId);
            if (user == null)
            {
                return DataResponse<UserDto>.Failure("User not found");
            }
            var result = user.UpdateProfilePictureURL(imageurl);
            if (!result.IsSuccess)
            {
                return DataResponse<UserDto>.Failure(result.Message);
            }
            var saveResult = await _uoW.SaveChangesAsync();
            if (saveResult <= 0)
            {
                return DataResponse<UserDto>.Failure("Failed to update profile picture");
            }
            var userDto = _mapper.Map<User, UserDto>(user);

            return DataResponse<UserDto>.Success(userDto);

        }
    

    }
}
