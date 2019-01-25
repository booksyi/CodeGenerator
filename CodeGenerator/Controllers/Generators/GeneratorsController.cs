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

        // /api/Generators/Generate
        [HttpPost("generate")]
        public async Task<ActionResult> GenerateByNode([FromBody] GenerateNode node)
        {
            return new OkObjectResult(await node.GenerateAsync());
        }

        // /api/Generators/{int}/Generate
        [HttpGet("{id:int}/generate")]
        public async Task<ActionResult> GenerateByRequestId([FromRoute] int id)
        {
            Generate.Request request = new Generate.Request()
            {
                Id = id,
                Body = Request.Query.ToJObject()
            };
            var resources = await mediator.Send(request);
            return new OkObjectResult(resources);
        }

        // /api/Generators/{int}/Generate
        [HttpPost("{id:int}/generate")]
        public async Task<ActionResult> GenerateByRequestId([FromRoute] int id, [FromBody] Generate.Request request)
        {
            request.Id = id;
            if (request.Body == null)
            {
                request.Body = await Request.Body.ToJObjectAsync();
            }
            var resources = await mediator.Send(request);
            return new OkObjectResult(resources);
        }

        [HttpPost("generate/download")]
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

        [HttpGet("{id:int}/generate/download")]
        public async Task<ActionResult> DownloadByRequestId([FromRoute] int id)
        {
            Generate.Request request = new Generate.Request()
            {
                Id = id,
                Body = Request.Query.ToJObject()
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

        [HttpPost("{id:int}/generate/download")]
        public async Task<ActionResult> DownloadByRequestId([FromRoute] int id, [FromBody] Generate.Request request)
        {
            request.Id = id;
            if (request.Body == null)
            {
                request.Body = await Request.Body.ToJObjectAsync();
            }
            var resources = await mediator.Send(request);
            string text = resources.FirstOrDefault().Text;
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);
            System.IO.MemoryStream stream = new System.IO.MemoryStream(bytes);
            return new FileStreamResult(stream, "text/plain")
            {
                FileDownloadName = $"{Guid.NewGuid().ToString("N")}.txt"
            };
        }

        // /api/Generators/Tree
        [HttpPost("tree")]
        public async Task<ActionResult> GetTreeByNode([FromBody] GenerateNode node)
        {
            return new OkObjectResult(await node.ToJTokenForReadAsync());
        }

        // /api/Generators/{int}/Tree
        [HttpGet("{id:int}/tree")]
        public async Task<ActionResult> GetTreeByRequestId([FromRoute] int id)
        {
            ToGenerateNodes.Request request = new ToGenerateNodes.Request()
            {
                Id = id,
                Body = Request.Query.ToJObject()
            };
            var nodes = await mediator.Send(request);
            var result = await Task.WhenAll(nodes.Select(async x => await x.ToJTokenForReadAsync()));
            return new OkObjectResult(result);
        }

        // /api/Generators/{int}/Tree
        [HttpPost("{id:int}/tree")]
        public async Task<ActionResult> GetTreeByRequestId([FromRoute] int id, [FromBody] ToGenerateNodes.Request request)
        {
            request.Id = id;
            if (request.Body == null)
            {
                request.Body = await Request.Body.ToJObjectAsync();
            }
            var nodes = await mediator.Send(request);
            var result = await Task.WhenAll(nodes.Select(async x => await x.ToJTokenForReadAsync()));
            return new OkObjectResult(result);
        }
    }
}