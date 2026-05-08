using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Mapping;
using ChatApp.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ChatApp.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddScoped<IUserServices, UserServices>();
            services.AddScoped<IAuthServices, AuthServices>();
            services.AddScoped<IChatServices, ChatServices>();
            return services;
        }
    }
}
