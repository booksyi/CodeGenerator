using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CodeGenerator.Controllers.Generators.Handlers;
using CodeGenerator.Controllers.RequestNodes.Handlers;
using HelpersForCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace CodeGenerator.Controllers.Generators
{
    [Route("api/[controller]")]
    public class GeneratorsController : Controller
    {
        private readonly IMediator mediator;

        public GeneratorsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("download")]
        public async Task<ActionResult> DownloadByNode([FromBody] GenerateNode node)
        {
            string text = await node.GenerateAsync();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);
            System.IO.MemoryStream stream = new System.IO.MemoryStream(bytes);
            return new FileStreamResult(stream, "text/plain")
            {
                FileDownloadName = $"{Guid.NewGuid().ToString("N")}.txt"
            };
        }

        [HttpPost("{id:int}/download")]
        public async Task<ActionResult> DownloadByRequestId([FromRoute] int id, [FromBody] Dictionary<string, JToken> body)
        {
            Generate.Request request = new Generate.Request()
            {
                Id = id,
                Body = body
            };
            var resources = await mediator.Send(request);
            string text = resources.FirstOrDefault().Text;
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);
            System.IO.MemoryStream stream = new System.IO.MemoryStream(bytes);
            return new FileStreamResult(stream, "text/plain")
            {
                FileDownloadName = $"{Guid.NewGuid().ToString("N")}.txt"
            };
        }

        // /api/Generators/Generate
        [HttpPost("generate")]
        public async Task<ActionResult> GenerateByNode([FromBody] GenerateNode node)
        {
            return new OkObjectResult(await node.GenerateAsync());
        }

        // /api/Generators/{int}/Generate
        [HttpPost("{id:int}/generate")]
        public async Task<ActionResult> GenerateByRequestId([FromRoute] int id, [FromBody] Dictionary<string, JToken> body)
        {
            Generate.Request request = new Generate.Request()
            {
                Id = id,
                Body = body
            };
            var resources = await mediator.Send(request);
            return new OkObjectResult(resources);
        }

        // /api/Generators/{int}/Tree
        [HttpPost("tree")]
        public async Task<ActionResult> GetTreeByNode([FromBody] GenerateNode node)
        {
            return new OkObjectResult(await node.ToJTokenAsync());
        }

        // /api/Generators/{int}/Tree
        [HttpPost("{id:int}/tree")]
        public async Task<ActionResult> GetTreeByRequestId([FromRoute] int id, [FromBody] Dictionary<string, JToken> body)
        {
            ToGenerateNodes.Request request = new ToGenerateNodes.Request()
            {
                Id = id,
                Body = body
            };
            var nodes = await mediator.Send(request);
            var result = await Task.WhenAll(nodes.Select(async x => await x.ToJTokenAsync()));
            return new OkObjectResult(result);
        }
    }
}