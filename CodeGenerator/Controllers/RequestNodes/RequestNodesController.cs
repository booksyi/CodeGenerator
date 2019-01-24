using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGenerator.Controllers.RequestNodes.Handlers;
using HelpersForCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace CodeGenerator.Controllers.RequestNodes
{
    [Route("api/[controller]")]
    public class RequestNodesController : Controller
    {
        private readonly IMediator mediator;

        public RequestNodesController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        // /api/RequestNodes/{int}/ToGenerateNodes
        [HttpGet("{id:int}/toGenerateNodes")]
        public async Task<ActionResult> ToGenerateNodes([FromRoute] int id)
        {
            ToGenerateNodes.Request request = new ToGenerateNodes.Request()
            {
                Id = id,
                Body = Request.Query.ToJObject()
            };
            var nodes = await mediator.Send(request);
            return new OkObjectResult(nodes);
        }

        // /api/RequestNodes/{int}/ToGenerateNodes
        [HttpPost("{id:int}/toGenerateNodes")]
        public async Task<ActionResult> ToGenerateNodes([FromRoute] int id, [FromBody] ToGenerateNodes.Request request)
        {
            request.Id = id;
            if (request.Body == null)
            {
                request.Body = await Request.Body.ToJObjectAsync();
            }
            var nodes = await mediator.Send(request);
            return new OkObjectResult(nodes);
        }

        [HttpPost("")]
        public async Task<ActionResult> CreateRequestNode([FromBody] CreateRequestNode.Request request)
        {
            if (request.Node == null)
            {
                request.Node = await Request.Body.ToClassAsync<RequestNode>();
            }
            int id = await mediator.Send(request);
            return new OkObjectResult(id);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateRequestNodeById([FromRoute] int id, [FromBody] UpdateRequestNodeById.Request request)
        {
            request.Id = id;
            if (request.Node == null)
            {
                request.Node = await Request.Body.ToClassAsync<RequestNode>();
            }
            await mediator.Send(request);
            return new OkResult();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetRequestNodeById([FromRoute] int id, [FromQuery] GetRequestNodeById.Request request)
        {
            request.Id = id;
            var node = await mediator.Send(request);
            return new OkObjectResult(node);
        }
    }
}