using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CodeGenerator.Data.Authentication;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeGenerator.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public UsersController(
            IMediator mediator,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            this.mediator = mediator;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public async Task<ActionResult> Login(string email, string password)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(e => e.NormalizedEmail.Equals(email, StringComparison.CurrentCultureIgnoreCase));
            var result = await signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);
            return Ok();
        }

        public async Task<ActionResult> Info()
        {
            var email = Request.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            return Content(email);
        }

    }
}