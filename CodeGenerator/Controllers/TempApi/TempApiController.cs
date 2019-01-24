using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGenerator.Controllers.TempApi.Handlers;
using CodeGenerator.Data.Models;
using HelpersForCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CodeGenerator.Controllers.TempApi
{
    [Route("api/temp")]
    public class TempApiController : Controller
    {
        private readonly IMediator mediator;

        public TempApiController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("")]
        public async Task<ActionResult> CreateTempApi([FromBody] CreateTempApi.Request request)
        {
            if (request == null)
            {
                request = new CreateTempApi.Request();
            }
            if (request.Result == null)
            {
                request.Result = await Request.Body.ToStringAsync();
            }
            int id = await mediator.Send(request);
            return new OkObjectResult(id);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateTempApiById([FromRoute] int id, [FromBody] UpdateTempApiById.Request request)
        {
            request.Id = id;
            if (request.Result == null)
            {
                request.Result = await Request.Body.ToStringAsync();
            }
            await mediator.Send(request);
            return new OkResult();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetTempApiById([FromRoute] int id, [FromQuery] GetTempApiById.Request request)
        {
            request.Id = id;
            var tempApi = await mediator.Send(request);
            return new OkObjectResult(tempApi);
        }

        [HttpPost("{id:int}/post")]
        public async Task<ActionResult> PostResult([FromRoute] int id, [FromBody] GetTempApiById.Request request)
        {
            request.Id = id;
            var tempApi = await mediator.Send(request);
            return Content(tempApi.Result);
        }

        [HttpGet("{id:int}/get")]
        public async Task<ActionResult> GetResult([FromRoute] int id, [FromQuery] GetTempApiById.Request request)
        {
            request.Id = id;
            var tempApi = await mediator.Send(request);
            return Content(tempApi.Result);
        }
    }
}