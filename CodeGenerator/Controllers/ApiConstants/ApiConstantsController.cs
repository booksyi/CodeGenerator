using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGenerator.Controllers.ApiConstants.Handlers;
using CodeGenerator.Data.Models;
using HelpersForCore;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodeGenerator.Controllers.ApiConstants
{
    [Route("api/[controller]")]
    public class ApiConstantsController : Controller
    {
        private readonly IMediator mediator;

        public ApiConstantsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("")]
        public async Task<ActionResult> CreateApiConstant([FromBody] CreateApiConstant.Request request)
        {
            DbApiConstant apiConstant = await mediator.Send(request);
            return new OkObjectResult(apiConstant);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateApiConstantById([FromRoute] int id, [FromBody] string result)
        {
            UpdateApiConstantById.Request request = new UpdateApiConstantById.Request()
            {
                Id = id,
                Result = result
            };
            DbApiConstant apiConstant = await mediator.Send(request);
            return new OkObjectResult(apiConstant);
        }

        [HttpGet("")]
        public async Task<ActionResult> GetApiConstants([FromQuery] GetApiConstants.Request request)
        {
            DbApiConstant[] apiConstants = await mediator.Send(request);
            return new OkObjectResult(apiConstants);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetApiConstantById([FromRoute] int id)
        {
            GetApiConstantById.Request request = new GetApiConstantById.Request
            {
                Id = id
            };
            DbApiConstant apiConstant = await mediator.Send(request);
            return new OkObjectResult(apiConstant);
        }

        [HttpGet("{id:int}/result")]
        [HttpPost("{id:int}/result")]
        public async Task<ActionResult> GetApiConstantResultById([FromRoute] int id)
        {
            GetApiConstantById.Request request = new GetApiConstantById.Request()
            {
                Id = id
            };
            DbApiConstant apiConstant = await mediator.Send(request);
            return Content(apiConstant.Result);
        }
    }
}