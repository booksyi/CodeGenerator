using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CodeGenerator.Controllers.Users.Handlers;
using CodeGenerator.Data;
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

        [HttpPost("[action]")]
        public async Task<ActionResult> Register([FromBody]Register.Request request, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (await mediator.Send(request))
                {
                    if (string.IsNullOrWhiteSpace(returnUrl))
                    {
                        return Redirect("~/");
                    }
                    return Redirect(returnUrl);
                }
            }
            return Ok(
                ModelState
                    .Where(x => x.Value.Errors.Any())
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value.Errors));
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> Login([FromBody]Login.Request request, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                // 驗證登入
                if (await mediator.Send(request))
                {
                    // 登入成功轉址
                    if (string.IsNullOrWhiteSpace(returnUrl))
                    {
                        return Redirect("~/");
                    }
                    return Redirect(returnUrl);
                }
                return Unauthorized();
            }
            return Ok(
                ModelState
                    .Where(x => x.Value.Errors.Any())
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value.Errors));
        }
    }
}