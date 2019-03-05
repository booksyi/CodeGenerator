using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGenerator.Controllers.Constants.Handlers;
using CodeGenerator.Data.Models;
using HelpersForCore;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodeGenerator.Controllers.Constants
{
    [Route("api/[controller]")]
    public class ConstantsController : Controller
    {
        private readonly IMediator mediator;

        public ConstantsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult> CreateOrUpdateConstant([FromRoute] int id, [FromBody] CreateOrUpdateConstant.Request request)
        {
            request.Id = id;
            Constant constant = await mediator.Send(request);
            return Ok(constant);
        }

        [HttpGet("")]
        public async Task<ActionResult> GetConstants([FromQuery] GetConstants.Request request)
        {
            Constant[] constants = await mediator.Send(request);
            return Ok(constants);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetConstantById([FromRoute] int id)
        {
            GetConstantById.Request request = new GetConstantById.Request
            {
                Id = id
            };
            Constant constant = await mediator.Send(request);
            return Ok(constant);
        }

        [HttpGet("{id:int}/result")]
        [HttpPost("{id:int}/result")]
        public async Task<ActionResult> GetConstantResultById([FromRoute] int id)
        {
            GetConstantById.Request request = new GetConstantById.Request()
            {
                Id = id
            };
            Constant constant = await mediator.Send(request);
            return Content(constant.Result);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteConstantById([FromRoute] int id)
        {
            DeleteConstantById.Request request = new DeleteConstantById.Request()
            {
                Id = id
            };
            await mediator.Send(request);
            return NoContent();
        }
    }
}