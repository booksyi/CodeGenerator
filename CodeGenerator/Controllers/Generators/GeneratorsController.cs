using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGenerator.Controllers.Generators.Handlers;
using CodeGenerator.Data.Models;
using HelpersForCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

        [HttpPost("")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult> CreateOrUpdateGenerator([FromRoute] int id, [FromBody] CreateOrUpdateGenerator.Request request)
        {
            request.Id = id;
            Generator generator = await mediator.Send(request);
            return Ok(generator);
        }

        [HttpGet("")]
        public async Task<ActionResult> GetGenerators([FromQuery] GetGenerators.Request request)
        {
            Generator[] generators = await mediator.Send(request);
            return Ok(generators);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetGeneratorById([FromRoute] int id)
        {
            GetGeneratorById.Request request = new GetGeneratorById.Request()
            {
                Id = id
            };
            Generator generator = await mediator.Send(request);
            return Ok(generator);
        }

        [HttpGet("{id:int}/codeTemplate")]
        public async Task<ActionResult> GetGeneratorCodeTemplateById([FromRoute] int id)
        {
            GetGeneratorById.Request request = new GetGeneratorById.Request()
            {
                Id = id
            };
            Generator generator = await mediator.Send(request);
            CodeTemplate codeTemplate = JsonConvert.DeserializeObject<CodeTemplate>(generator.Json);
            return Ok(codeTemplate);
        }

        [HttpGet("{id:int}/inputs")]
        public async Task<ActionResult> GetGeneratorInputsById([FromRoute] int id)
        {
            GetGeneratorById.Request request = new GetGeneratorById.Request()
            {
                Id = id
            };
            Generator generator = await mediator.Send(request);
            CodeTemplate codeTemplate = JsonConvert.DeserializeObject<CodeTemplate>(generator.Json);
            return Ok(codeTemplate.Inputs);
        }

        [HttpGet("{id:int}/templates")]
        public async Task<ActionResult> GetGeneratorTemplatesById([FromRoute] int id)
        {
            GetGeneratorById.Request request = new GetGeneratorById.Request()
            {
                Id = id
            };
            Generator generator = await mediator.Send(request);
            CodeTemplate codeTemplate = JsonConvert.DeserializeObject<CodeTemplate>(generator.Json);
            return Ok(codeTemplate.GetTemplateUris());
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteGeneratorById([FromRoute] int id)
        {
            DeleteGeneratorById.Request request = new DeleteGeneratorById.Request()
            {
                Id = id
            };
            await mediator.Send(request);
            return NoContent();
        }

        // /api/Generators/{int}/ToGenerateNodes
        [HttpGet("{id:int}/toGenerateNodes")]
        public async Task<ActionResult> ToGenerateNodes([FromRoute] int id)
        {
            ToGenerateNodes.Request request = new ToGenerateNodes.Request()
            {
                Id = id,
                Body = Request.Query.ToJObject()
            };
            var nodes = await mediator.Send(request);
            return Ok(nodes);
        }

        // /api/Generators/{int}/ToGenerateNodes
        [HttpPost("{id:int}/toGenerateNodes")]
        public async Task<ActionResult> ToGenerateNodes([FromRoute] int id, [FromBody] ToGenerateNodes.Request request)
        {
            request.Id = id;
            if (request.Body == null)
            {
                request.Body = await Request.Body.ToJTokenAsync<JObject>();
            }
            var nodes = await mediator.Send(request);
            return Ok(nodes);
        }

        // /api/Generators/Generate
        [HttpPost("generate")]
        public async Task<ActionResult> GenerateByNode([FromBody] GenerateNode node)
        {
            return Ok(await node.GenerateAsync());
        }

        // /api/Generators/{int}/Generate
        [HttpGet("{id:int}/generate")]
        public async Task<ActionResult> GenerateById([FromRoute] int id)
        {
            Generate.Request request = new Generate.Request()
            {
                Id = id,
                Body = Request.Query.ToJObject()
            };
            var resources = await mediator.Send(request);
            return Ok(resources);
        }

        // /api/Generators/{int}/Generate
        [HttpPost("{id:int}/generate")]
        public async Task<ActionResult> GenerateById([FromRoute] int id, [FromBody] Generate.Request request)
        {
            request.Id = id;
            if (request.Body == null)
            {
                request.Body = await Request.Body.ToJTokenAsync<JObject>();
            }
            var resources = await mediator.Send(request);
            return Ok(resources);
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
        public async Task<ActionResult> DownloadById([FromRoute] int id)
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
        public async Task<ActionResult> DownloadById([FromRoute] int id, [FromBody] Generate.Request request)
        {
            request.Id = id;
            if (request.Body == null)
            {
                request.Body = await Request.Body.ToJTokenAsync<JObject>();
            }
            var resources = await mediator.Send(request);
            string text = resources.FirstOrDefault().Text;  // TODO: 支援下載所有檔案
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
            return Ok(await node.ToJTokenForReadAsync());
        }

        // /api/Generators/{int}/Tree
        [HttpGet("{id:int}/tree")]
        public async Task<ActionResult> GetTreeById([FromRoute] int id)
        {
            ToGenerateNodes.Request request = new ToGenerateNodes.Request()
            {
                Id = id,
                Body = Request.Query.ToJObject()
            };
            var nodes = await mediator.Send(request);
            var result = await Task.WhenAll(nodes.Select(async x => await x.ToJTokenForReadAsync()));
            return Ok(result);
        }

        // /api/Generators/{int}/Tree
        [HttpPost("{id:int}/tree")]
        public async Task<ActionResult> GetTreeById([FromRoute] int id, [FromBody] ToGenerateNodes.Request request)
        {
            request.Id = id;
            if (request.Body == null)
            {
                request.Body = await Request.Body.ToJTokenAsync<JObject>();
            }
            var nodes = await mediator.Send(request);
            var result = await Task.WhenAll(nodes.Select(async x => await x.ToJTokenForReadAsync()));
            return Ok(result);
        }
    }
}