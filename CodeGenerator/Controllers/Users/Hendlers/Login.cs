using CodeGenerator.Data.Authentication;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.Users.Hendlers
{
    public class Login
    {
        public class Request : IRequest<bool>
        {
            public string Email { get; set; }
            public string Password { get; set; }
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
                ApplicationUser user = await userManager.Users.FirstOrDefaultAsync(e => e.NormalizedEmail.Equals(request.Email, StringComparison.CurrentCultureIgnoreCase));
                SignInResult result = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
                return result.Succeeded;
            }
        }
    }
}
