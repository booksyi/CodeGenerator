using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGenerator.Controllers.CodeTemplates.Handlers;
using CodeGenerator.Data.Models;
using HelpersForCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CodeGenerator.Controllers.CodeTemplates
{
    [Route("api/[controller]")]
    [ApiController]
    public class CodeTemplatesController : ControllerBase
    {
        private readonly IMediator mediator;

        public CodeTemplatesController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        // /api/CodeTemplates/{int}/ToGenerateNodes
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

        // /api/CodeTemplates/{int}/ToGenerateNodes
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
        public async Task<ActionResult> CreateCodeTemplate([FromBody] CodeTemplate template)
        {
            CreateCodeTemplate.Request request = new CreateCodeTemplate.Request()
            {
                Template = template
            };
            DbCodeTemplate codeTemplate = await mediator.Send(request);
            return new OkObjectResult(codeTemplate);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateCodeTemplateById([FromRoute] int id, [FromBody] CodeTemplate template)
        {
            UpdateCodeTemplateById.Request request = new UpdateCodeTemplateById.Request()
            {
                Id = id,
                Template = template
            };
            DbCodeTemplate codeTemplate = await mediator.Send(request);
            return new OkObjectResult(codeTemplate);
        }

        [HttpGet("")]
        public async Task<ActionResult> GetCodeTemplates([FromQuery] GetCodeTemplates.Request request)
        {
            DbCodeTemplate[] codeTemplates = await mediator.Send(request);
            return new OkObjectResult(codeTemplates);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetCodeTemplateById([FromRoute] int id)
        {
            GetCodeTemplateById.Request request = new GetCodeTemplateById.Request()
            {
                Id = id
            };
            DbCodeTemplate codeTemplate = await mediator.Send(request);
            return new OkObjectResult(codeTemplate);
        }

        [HttpGet("{id:int}/template")]
        public async Task<ActionResult> GetCodeTemplateTemplateById([FromRoute] int id)
        {
            GetCodeTemplateById.Request request = new GetCodeTemplateById.Request()
            {
                Id = id
            };
            DbCodeTemplate codeTemplate = await mediator.Send(request);
            CodeTemplate template = JsonConvert.DeserializeObject<CodeTemplate>(codeTemplate.Node);
            return new OkObjectResult(template);
        }

        [HttpGet("{id:int}/inputs")]
        public async Task<ActionResult> GetCodeTemplateInputsById([FromRoute] int id)
        {
            GetCodeTemplateById.Request request = new GetCodeTemplateById.Request()
            {
                Id = id
            };
            DbCodeTemplate codeTemplate = await mediator.Send(request);
            CodeTemplate template = JsonConvert.DeserializeObject<CodeTemplate>(codeTemplate.Node);
            return new OkObjectResult(template.Inputs);
        }

        [HttpGet("{id:int}/templates")]
        public async Task<ActionResult> GetCodeTemplateTemplatesById([FromRoute] int id)
        {
            GetCodeTemplateById.Request request = new GetCodeTemplateById.Request()
            {
                Id = id
            };
            DbCodeTemplate codeTemplate = await mediator.Send(request);
            CodeTemplate template = JsonConvert.DeserializeObject<CodeTemplate>(codeTemplate.Node);
            return new OkObjectResult(template.GetTemplateUris());
        }

        [HttpGet("dev")]
        public async Task<ActionResult> GetRequestNodeJsonFromDeveloper()
        {
            var json = await mediator.Send(new GetCodeTemplateFromDeveloper.Request());
            return Content(json);
        }
    }
}