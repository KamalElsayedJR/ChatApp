using ChatApp.Application.Dtos;
using ChatApp.Application.Dtos.Image;
using ChatApp.Application.Dtos.User;
using ChatApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatApp.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAuthServices _authServices;
        private readonly IUserServices _userServices;

        public UserController(IAuthServices authServices, IUserServices userServices)
        {
            _authServices = authServices;
            _userServices = userServices;
        }
        [HttpGet("profile")]
        public async Task<ActionResult<DataResponse<UserDto>>> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Unauthorized(DataResponse<UserDto>.Failure("Unauthorized please login again"));
            }
            var result = await _authServices.Profile(userId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPut("update-bio")]
        public async Task<ActionResult<DataResponse<UserDto>>> UpdateBio([FromForm] string bio)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Unauthorized(DataResponse<UserDto>.Failure("Unauthorized please login again"));
            }
            var result = await _userServices.UpdateBio(userId, bio);
            return result.IsSuccess ? Ok(result) : BadRequest(result);

        }
        [HttpPut("update-email")]
        public async Task<ActionResult<DataResponse<UserDto>>> UpdateEmail([FromForm] string email)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Unauthorized(DataResponse<UserDto>.Failure("Unauthorized please login again"));
            }
            var result = await _userServices.UpdateEmail(userId, email);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        
        [HttpPut("update-profile-picture")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<DataResponse<UserDto>>> UpdateProfilePicture([FromForm] UpdateProfilePictureRequest file)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Unauthorized(DataResponse<UserDto>.Failure("Unauthorized please login again"));
            }
            var result = await _userServices.UpdateProfilePicture(userId, file.File);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        
        [HttpPut("update-full-name")]
        public async Task<ActionResult<DataResponse<UserDto>>> UpdateFullName([FromForm] string fullName)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Unauthorized(DataResponse<UserDto>.Failure("Unauthorized please login again"));
            }
            var result = await _userServices.UpdateFullName(userId, fullName);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("search")]
        public async Task<ActionResult<DataResponse<List<SearchedUsers>>>> SearchUsers([FromQuery] string query)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Unauthorized(DataResponse<List<SearchedUsers>>.Failure("Unauthorized please login again"));
            }
            var result = await _userServices.SearchUsers(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("GetUserById")]
        public async Task<ActionResult<DataResponse<SearchedUsers>>> GetUserById([FromQuery] string userId)
        {
            var result = await _userServices.GetUserById(userId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
