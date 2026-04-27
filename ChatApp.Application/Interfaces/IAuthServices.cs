using ChatApp.Application.Dtos;
using ChatApp.Application.Dtos.Auth;
using ChatApp.Domain.Common;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Interfaces
{
    public interface IAuthServices
    {
        Task<Result> RegisterAsync(RegisterRequest request);
        Task<DataResponse<LoginResposnse>> LoginAsync(LoginRequset requset);
        Task<string?> GenerateRefreshTokenAsync(string UserId);
        Task<string?> RefreshTokenAsync(string RefreshToken, string ExpirdAccessToken);
        Task<Result> LogoutAsync(string UserId);
        Task<Result> SendOtpAsync(string Email, OtpType otpType);
        Task<Result> VerifyOtpAsync(VerifyOtpRequest request, OtpType otpType);
        Task<Result> ResetPasswordAsync(User user, ChangePasswordRequest request);
        Task<DataResponse<UserDto>> Profile(string userId);
    }
}
