using ChatApp.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Domain.Entities
{
    public class User
    {
        public string Id { get; private set; } = Guid.NewGuid().ToString();
        public string FullName { get; private set; } = string.Empty;
        public string UserName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string HashedPassword { get; private set; } = string.Empty;
        public string ProfilePictureURL { get; private set; } = string.Empty;
        public string Bio { get; private set; } = string.Empty;
        public bool IsEmailConfirmed { get; private set; } = false;
        public bool IsActive { get; private set; } = true;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public ICollection<UserOtp> UserOtps { get; set; } = new List<UserOtp>();
        private User() { }
        public User(string fullName, string userName, string email, string hashedPassword)
        {
            FullName = fullName;
            UserName = userName;
            Email = email;
            HashedPassword = hashedPassword;
        }
        public Result UpdateFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return Result.Failure("Full name cannot be empty.");
            FullName = fullName.Trim();
            UpdatedAt = DateTime.UtcNow;
            return Result.Success("Full name updated successfully.");
        }
        public Result UpdateBio(string bio)
        {
            Bio = bio?.Trim() ?? string.Empty;
            UpdatedAt = DateTime.UtcNow;
            return Result.Success("Bio updated successfully.");
        }
        public Result UpdateProfilePictureURL(string profilePictureURL)
        {
            ProfilePictureURL = profilePictureURL;
            UpdatedAt = DateTime.UtcNow;
            return Result.Success("Profile picture updated successfully.");
        }
        public Result ChangePassword(string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(hashedPassword))
                return Result.Failure("Password cannot be empty.");
            HashedPassword = hashedPassword;
            UpdatedAt = DateTime.UtcNow;
            return Result.Success("Password updated successfully.");
        }
        public Result ConfirmEmail()
        {
            if (IsEmailConfirmed)
                return Result.Failure("Email is already confirmed.");
            IsEmailConfirmed = true;
            UpdatedAt = DateTime.UtcNow;
            return Result.Success("Email confirmed successfully.");
        }
        public Result Deactivate()
        {
            if (!IsActive)
                return Result.Failure("User is already deactivated.");
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
            return Result.Success("User deactivated successfully.");
        }
        public Result Activate()
        {
            if (IsActive)
                return Result.Failure("User is already active.");
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
            return Result.Success("User activated successfully.");
        }
        public Result ChangeEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return Result.Failure("Email cannot be empty.");
            if(email.Trim().ToLower() == Email.ToLower())
                return Result.Failure("New email cannot be the same as the current email.");
            Email = email.Trim().ToLower();
            IsEmailConfirmed = false; // Require reconfirmation for new email
            UpdatedAt = DateTime.UtcNow;
            return Result.Success("Email updated successfully. Please confirm your new email.");
        }

    }
}
