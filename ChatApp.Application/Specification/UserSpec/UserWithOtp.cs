using ChatApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Specification.UserSpec
{
    public class UserWithOtp : BaseSpecification<User>
    {
        public UserWithOtp(string email) : base(u => u.Email == email)
        {
            Includes.Add(u => u.UserOtps);
        }
    }
}
