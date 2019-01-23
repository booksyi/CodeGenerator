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
        [HttpPost("{id:int}/[action]")]
        public async Task<ActionResult> ToGenerateNodes([FromRoute] int id, [FromBody] Dictionary<string, JToken> body)
        {
            ToGenerateNodes.Request request = new ToGenerateNodes.Request()
            {
                Id = id,
                Body = body
            };
            var nodes = await mediator.Send(request);
            return new OkObjectResult(nodes);
        }

        [HttpPost("")]
        public async Task<ActionResult> Create([FromBody] RequestNode node)
        {
            CreateRequestNode.Request request = new CreateRequestNode.Request()
            {
                Node = node
            };
            int id = await mediator.Send(request);
            return new OkObjectResult(id);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateById([FromRoute] int id, [FromBody] RequestNode node)
        {
            UpdateRequestNodeById.Request request = new UpdateRequestNodeById.Request()
            {
                Id = id,
                Node = node
            };
            await mediator.Send(request);
            return new OkResult();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetById([FromRoute] int id)
        {
            GetRequestNodeById.Request request = new GetRequestNodeById.Request()
            {
                Id = id
            };
            var node = await mediator.Send(request);
            return new OkObjectResult(node);
        }
    }
}