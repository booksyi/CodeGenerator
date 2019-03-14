using CodeGenerator.Data;
using CodeGenerator.Data.Authentication;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.Users.Hendlers
{
    public class Register
    {
        public class Request : IRequest<bool>
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class Exception : ApiException
        {
            public IEnumerable<IdentityError> Errors { get; set; }
            public override object ThrowObject => new { Errors };
        }

        public class Handler : IRequestHandler<Request, bool>
        {
            private readonly UserManager<ApplicationUser> userManager;
            private readonly SignInManager<ApplicationUser> signInManager;

            public Handler(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
            {
                this.userManager = userManager;
                this.signInManager = signInManager;
            }

            public async Task<bool> Handle(Request request, CancellationToken token)
            {
                ApplicationUser user = new ApplicationUser()
                {
                    UserName = request.Name,
                    Email = request.Email,
                };
                user.PasswordHash = userManager.PasswordHasher.HashPassword(user, request.Password);
                IdentityResult result = await userManager.CreateAsync(user);
                if (result.Succeeded == false)
                {
                    throw new Exception() { Errors = result.Errors };
                }
                return true;
            }
        }
    }
}
