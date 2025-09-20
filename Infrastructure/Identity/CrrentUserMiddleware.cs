using Application.Features.Identity.Users;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity
{
    public class CrrentUserMiddleware : IMiddleware
    {
        private readonly ICurrentUSerService _currentUser;

        public CrrentUserMiddleware(ICurrentUSerService currentUser)
        {
            _currentUser=currentUser;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            _currentUser.SetCurrentUser(context.User);
            await next(context);
        }
    }
}
