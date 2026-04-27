using ChatApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Specification.UserSpec
{
    public class AllUsersWithCondition : BaseSpecification<User>
    {
        public AllUsersWithCondition(string name):base(u => u.FullName.Contains(name)|| u.UserName.Contains(name))
        {
            
        }
    }
}
