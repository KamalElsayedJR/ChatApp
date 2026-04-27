using ChatApp.Application.Dtos;
using ChatApp.Application.Dtos.Auth;
using ChatApp.Application.Dtos.Mail;
using ChatApp.Application.Interfaces;
using ChatApp.Domain.Common;
using ChatApp.Domain.Enums;
using ChatApp.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace ChatApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authServices;
        public AuthController(IAuthServices authServices)
        {
            _authServices = authServices;
        }
        [HttpPost("register")]
        public async Task<ActionResult<Result>> Register([FromBody] RegisterRequest request)
        {
            var result = await _authServices.RegisterAsync(request);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("login")]
        public async Task<ActionResult<DataResponse<LoginResposnse>>> Login([FromBody] LoginRequset requset)
        {
            var result = await _authServices.LoginAsync(requset);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("refresh-token")]
        public async Task<ActionResult<DataResponse<string>>> RefreshToken(string RefreshToken, string ExpiredAccessToken)
        {
            var result = await _authServices.RefreshTokenAsync(RefreshToken, ExpiredAccessToken);
            if (result is null)
            {
                return Unauthorized(DataResponse<string>.Failure("Unauthorized please login again"));
            }
            return Ok(DataResponse<string>.Success(result));
        }
        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult<Result>> Logout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Unauthorized(Result.Failure("Unauthorized please login again"));
            }
            var result = await _authServices.LogoutAsync(userId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("request-confirm-email-otp")]
        public async Task<ActionResult<Result>> SendConfirmEmailOtp(SendOtpRequest email)
        {
            var result = await _authServices.SendOtpAsync(email.Email, OtpType.EmailConfirmation);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("confirm-email")]
        public async Task<ActionResult<Result>> VerifyOtp(VerifyOtpRequest request)
        {
            var result = await _authServices.VerifyOtpAsync(request, OtpType.EmailConfirmation);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("request-password-reset-otp")]
        public async Task<ActionResult<Result>> SendPasswordResetOtp(SendOtpRequest email)
        {
            var result = await _authServices.SendOtpAsync(email.Email, OtpType.PasswordReset);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("reset-password")]
        public async Task<ActionResult<Result>> ResetPassword(VerifyOtpRequest request)
        {
            var result = await _authServices.VerifyOtpAsync(request, OtpType.PasswordReset);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        //[Authorize]
        //[HttpPost("change-password")]
        //public async Task<ActionResult<Result>> ChangePassword(ChangePasswordRequest request)
        //{
        //    var result = await _authServices.ChangePasswordAsync(request);
        //    return result.IsSuccess ? Ok(result) : BadRequest(result);
        //}
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
    }
}