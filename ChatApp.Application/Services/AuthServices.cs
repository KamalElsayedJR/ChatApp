using ChatApp.Application.Dtos;
using ChatApp.Application.Dtos.Auth;
using ChatApp.Application.Dtos.Mail;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Repositories;
using ChatApp.Application.Specification;
using ChatApp.Application.Specification.UserSpec;
using ChatApp.Domain.Common;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ChatApp.Application.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly IPasswordServices _passwordServices;
        private readonly IUnitOfWork _uoW;
        private readonly IJWTServices _jWTServices;
        private readonly IEmailServices _emailServices;

        public AuthServices(IPasswordServices passwordServices, IUnitOfWork UoW,
                            IJWTServices jWTServices, IEmailServices emailServices)
        {
            _passwordServices = passwordServices;
            _uoW = UoW;
            _jWTServices = jWTServices;
            _emailServices = emailServices;
        }
        public async Task<string?> GenerateRefreshTokenAsync(string UserId)
        {
            if (string.IsNullOrEmpty(UserId))
                return null;
            var refreshToken = await _uoW.RefreshTokenRepository.GetRefreshTokenForUserAsync(UserId);
            if (refreshToken is not null)
            {
                if (refreshToken.ExpiredAt > DateTime.UtcNow && !refreshToken.IsRevoked)
                {
                    refreshToken.IsRevoked = true;
                    return refreshToken.Token;
                }
            }
            var rrefreshToken = Guid.NewGuid().ToString();
            await _uoW.Repository<RefreshToken>().AddAsync(new RefreshToken(rrefreshToken, DateTime.UtcNow.AddDays(7), UserId));
            var result = await _uoW.SaveChangesAsync();
            if (result <= 0)
                return null;
            return rrefreshToken;
        }
        public async Task<DataResponse<LoginResposnse>> LoginAsync(LoginRequset requset)
        {
            var user = await _uoW.UserRepository.GetUserByEmailAsync(requset.Email);
            if (user is null)
            {
                return DataResponse<LoginResposnse>.Failure("Invalid email or password");
            }

            if (!_passwordServices.VerifyPassword(requset.Password, user.HashedPassword))
            {
                return DataResponse<LoginResposnse>.Failure("Invalid email or password");
            }
            if (!user.IsActive)
            {
                return DataResponse<LoginResposnse>.Failure("User account is inactive");
            }
            if (!user.IsEmailConfirmed)
            {
                var result = await SendOtpAsync(user.Email, OtpType.EmailConfirmation);
                if (!result.IsSuccess)
                {
                    return DataResponse<LoginResposnse>.Failure("Email is not confirmed,Failed to send confirmation OTP");
                }
                return DataResponse<LoginResposnse>.Failure("Email is not confirmed,Check your email for confirmation otp");
            }
            var AccessToken = _jWTServices.GenerateJWTAccessToken(user);
            if (AccessToken is null)
            {
                return DataResponse<LoginResposnse>.Failure("Failed to Login");
            }
            var refreshTokens = await GenerateRefreshTokenAsync(user.Id);
            if (refreshTokens is null)
            {
                return DataResponse<LoginResposnse>.Failure("Failed to Login");
            }
            var logedInUser = new LoginResposnse
            {
                UserName = user.UserName,
                ProfilePictureURL = user.ProfilePictureURL,
                AccessToken = AccessToken,
                RefreshToken = refreshTokens
            };
            return DataResponse<LoginResposnse>.Success(logedInUser);
        }
        public async Task<Result> RegisterAsync(RegisterRequest request)
        {
            request.Email = request.Email.Trim().ToLower();
            var repo = _uoW.Repository<User>();
            var existEmail = await repo.AnyAsync(u => u.Email == request.Email);
            if (existEmail)
                return Result.Failure("Email already exists");
            request.Username = request.Username.Trim().ToLower();
            var existUsername = await repo.AnyAsync(u => u.UserName == request.Username);
            if (existUsername)
                return Result.Failure("Username already exists");
            var user = new User(request.FullName, request.Username, request.Email,
                                _passwordServices.HashPassword(request.Password));
            await repo.AddAsync(user);
            var result = await _uoW.SaveChangesAsync();
            if (result > 0)
                return Result.Success("User registered successfully");

            return Result.Failure("Failed to register user");
        }
        public async Task<string?> RefreshTokenAsync(string RefreshToken, string ExpirdAccessToken)
        {
            var principal = _jWTServices.GetPrincipalFromExpiredToken(ExpirdAccessToken);
            var UserId = principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(UserId))
                return null;
            var refreshToken = await _uoW.RefreshTokenRepository.GetRefreshTokenForUserAsync(UserId);
            if (refreshToken is null || refreshToken.ExpiredAt <= DateTime.UtcNow || refreshToken.IsRevoked)
                return null;
            if (refreshToken.Token != RefreshToken)
                return null;
            var UserEmail = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var user = await _uoW.UserRepository.GetUserByEmailAsync(UserEmail);
            var newAccessToken = _jWTServices.GenerateJWTAccessToken(user);
            if (newAccessToken is null)
                return null;
            return newAccessToken;
        }
        public async Task<Result> LogoutAsync(string UserId)
        {
            var refreshToken = await _uoW.RefreshTokenRepository.GetRefreshTokenForUserAsync(UserId);
            if (refreshToken is null)
                return Result.Failure("User is not logged in");
            refreshToken.IsRevoked = true;
            _uoW.Repository<RefreshToken>().Delete(refreshToken);
            var result = await _uoW.SaveChangesAsync();
            if (result <= 0)
                return Result.Failure("Failed to log out user");
            return Result.Success("User logged out successfully");
        }
        public async Task<Result> SendOtpAsync(string Email, OtpType otpType)
        {
            var existingUser = await _uoW.UserRepository.GetUserByEmailAsync(Email);
            if (existingUser is null)
            {
                return Result.Failure("Email not found");
            }
            var random = new Random();
            var otp = random.Next(100000, 999999);
            var userOtp = new UserOtp(otp, otpType, existingUser.Id);
            await _uoW.Repository<UserOtp>().AddAsync(userOtp);
            if (otpType == OtpType.EmailConfirmation && existingUser.IsEmailConfirmed)
            {
                return Result.Failure("Email is already confirmed");
            }
            #region EmailBody
            string url = otpType == OtpType.EmailConfirmation
        ? "https://localhost:7210/api/auth/confirm-email"
        : "https://localhost:7210/api/auth/reset-password";
            string emailPurpose = otpType == OtpType.EmailConfirmation ? "Email Confirmation" : "Password Reset";
            string purposeDetails = otpType == OtpType.EmailConfirmation
                ? "Thank you for registering with us! To complete your registration, please use the OTP code below to confirm your email address."
                : "We received a request to reset your password. Please use the OTP code below to proceed with resetting your password.";
            string details = otpType == OtpType.EmailConfirmation
                ? "If you didn't create an account with us, please ignore this email."
                : "If you didn't request a password reset, please ignore this email.";
            var body = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>{emailPurpose} - ChatApp</title>

    <style>
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }}

        body {{
            font-family: Arial, sans-serif;
            background: linear-gradient(135deg, #eef3ff, #f8faff);
            padding: 30px 15px;
            color: #1f2937;
        }}

        .wrapper {{
            max-width: 650px;
            margin: auto;
        }}

        .logo {{
            text-align: center;
            font-size: 42px;
            font-weight: bold;
            color: #0d6efd;
            margin-bottom: 25px;
        }}

        .container {{
            background: #ffffff;
            border-radius: 20px;
            padding: 40px 35px;
            box-shadow: 0 15px 40px rgba(0,0,0,0.08);
            overflow: hidden;
        }}

        .icon-box {{
            width: 90px;
            height: 90px;
            background: #eef4ff;
            border-radius: 50%;
            margin: 0 auto 25px;
            text-align: center;
            line-height: 90px;
            font-size: 40px;
        }}

        .title {{
            text-align: center;
            font-size: 34px;
            font-weight: bold;
            margin-bottom: 10px;
            color: #111827;
        }}

        .line {{
            width: 70px;
            height: 4px;
            background: #0d6efd;
            border-radius: 10px;
            margin: 0 auto 30px;
        }}

        .content p {{
            font-size: 18px;
            line-height: 1.8;
            margin-bottom: 18px;
            color: #374151;
        }}

        .otp-code {{
            background: #f8fbff;
            border: 2px solid #9fc2ff;
            border-radius: 16px;
            text-align: center;
            font-size: 52px;
            font-weight: bold;
            letter-spacing: 12px;
            padding: 28px 15px;
            margin: 30px 0;
            color: #0d6efd;
        }}

        .warning {{
            background: #fff9e8;
            border: 1px solid #ffe08a;
            color: #7c5a00;
            padding: 18px;
            border-radius: 14px;
            font-size: 16px;
            margin-bottom: 25px;
        }}

        .security {{
            font-size: 17px;
            margin-bottom: 30px;
            color: #374151;
        }}

        .btn {{
            display: block;
            width: 100%;
            text-align: center;
            text-decoration: none;
            background: linear-gradient(90deg, #0d6efd, #0056d6);
            color: white !important;
            padding: 18px;
            font-size: 20px;
            font-weight: bold;
            border-radius: 14px;
            margin-bottom: 35px;
        }}

        .note {{
            text-align: center;
            color: #6b7280;
            font-size: 16px;
            margin-bottom: 30px;
        }}

        .footer {{
            border-top: 1px solid #e5e7eb;
            padding-top: 25px;
            text-align: center;
            font-size: 15px;
            color: #6b7280;
        }}

        .footer a {{
            color: #0d6efd;
            text-decoration: none;
            font-weight: bold;
        }}

        .copyright {{
            margin-top: 18px;
            font-size: 14px;
        }}

        @media(max-width:600px) {{
            .container {{
                padding: 30px 20px;
            }}

            .otp-code {{
                font-size: 38px;
                letter-spacing: 8px;
            }}

            .title {{
                font-size: 28px;
            }}
        }}
    </style>
</head>

<body>
    <div class='wrapper'>

        <div class='logo'>ChatApp</div>

        <div class='container'>

            <div class='icon-box'>📩</div>

            <div class='title'>{emailPurpose}</div>

            <div class='line'></div>

            <div class='content'>

                <p>Hello {existingUser.FullName},</p>

                <p>
                    {purposeDetails}
                </p>

                <p>
                    {details}
                </p>

                <div class='otp-code'>
                    {otp}
                </div>

                <div class='warning'>
                    <strong>Important:</strong> This code will expire in {userOtp.ExpirationTime} minutes.
                </div>

                <p class='security'>
                    For your security, please do not share this OTP with anyone.
                </p>

                <a class='btn' href='{url}'>Click here to proceed</a>

                <p class='note'>
                    If you didn't request this action, please ignore this email.
                </p>

            </div>

            <div class='footer'>
                <p>
                    Need help?
                    <a href='mailto:kamal0elsayed0@gmail.com'>Contact our support team</a>
                </p>

                <p class='copyright'>
                    &copy; 2026 ChatApp. All rights reserved.
                </p>
            </div>

        </div>

    </div>
</body>
</html>";
            #endregion

            var email = new Dtos.Mail.Email
            {
                To = Email,
                Subject = otpType == OtpType.EmailConfirmation ? "Email Confirmation OTP" : "Password Reset OTP",
                Body = body
            };
            _emailServices.SendEmail(email);
            var result = await _uoW.SaveChangesAsync();
            if (result > 0)
            {
                return Result.Success("OTP sent successfully");
            }
            return Result.Failure("Failed to send OTP");
        }
        public async Task<Result> VerifyOtpAsync(VerifyOtpRequest request, OtpType otpType)
        {
            var spec = new UserWithOtp(request.Email);
            var user = await _uoW.Repository<User>().GetOneWithSpecAsync(spec);
            if (user == null)
            {
                return Result.Failure("User not found");
            }
            if (user.UserOtps == null || !user.UserOtps.Any(otp => otp.Otp == request.Otp && otp.OtpType == otpType))
            {
                return Result.Failure("Invalid OTP");
            }
            if (otpType == OtpType.EmailConfirmation)
            {
                user.ConfirmEmail();
            }
            if (otpType == OtpType.PasswordReset)
            {
                // Handle password reset logic here, such as allowing the user to set a new password
                
            }
            var result = await _uoW.SaveChangesAsync();
            if (result <= 0)
            {
                return Result.Failure("Failed to verify OTP");
            }
            return Result.Success("OTP verified successfully");
        }
        public async Task<Result> ResetPasswordAsync(User user, ChangePasswordRequest request)
        {
            if (user == null)
            {
                return Result.Failure("User not found");
            }
            if (request.NewPassword != request.ConfirmPassword)
            {
                return Result.Failure("Passwords do not match");
            }
            user.ChangePassword(_passwordServices.HashPassword(request.NewPassword));
            var result = await _uoW.SaveChangesAsync();
            if (result <= 0)
            {
                return Result.Failure("Failed to reset password");
            }
            return Result.Success("Password reset successfully");
        }
        public async Task<DataResponse<UserDto>> Profile(string userId)
        {
            var user = await _uoW.Repository<User>().GetByIdAsync(userId);
            if (user == null)
            {
                return DataResponse<UserDto>.Failure("User not found");
            }
            var userDto = new UserDto
            {
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email,
                ProfilePictureURL = user.ProfilePictureURL,
                Bio = user.Bio,
                IsEmailConfirmed = user.IsEmailConfirmed,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
            return DataResponse<UserDto>.Success(userDto);

        }
    }
}
