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
        public async Task<ActionResult> CreateConstant([FromBody] CreateConstant.Request request)
        {
            Constant apiConstant = await mediator.Send(request);
            return new OkObjectResult(apiConstant);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateConstantById([FromRoute] int id, [FromBody] UpdateConstantById.Request request)
        {
            request.Id = id;
            Constant apiConstant = await mediator.Send(request);
            return new OkObjectResult(apiConstant);
        }

        [HttpGet("")]
        public async Task<ActionResult> GetConstants([FromQuery] GetConstants.Request request)
        {
            Constant[] apiConstants = await mediator.Send(request);
            return new OkObjectResult(apiConstants);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetConstantById([FromRoute] int id)
        {
            GetConstantById.Request request = new GetConstantById.Request
            {
                Id = id
            };
            Constant apiConstant = await mediator.Send(request);
            return new OkObjectResult(apiConstant);
        }

        [HttpGet("{id:int}/result")]
        [HttpPost("{id:int}/result")]
        public async Task<ActionResult> GetConstantResultById([FromRoute] int id)
        {
            GetConstantById.Request request = new GetConstantById.Request()
            {
                Id = id
            };
            Constant apiConstant = await mediator.Send(request);
            return Content(apiConstant.Result);
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