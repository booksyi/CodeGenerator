using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGenerator.Controllers.Templates.Handlers;
using CodeGenerator.Data.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CodeGenerator.Controllers.Templates
{
    [Route("api/[controller]")]
    public class TemplatesController : Controller
    {
        private readonly IMediator mediator;

        public TemplatesController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("")]
        public async Task<ActionResult> CreateTemplate([FromBody] CreateTemplate.Request request)
        {
            int id = await mediator.Send(request);
            return new OkObjectResult(id);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateTemplateById([FromRoute] int id, [FromBody] UpdateTemplateById.Request request)
        {
            request.Id = id;
            await mediator.Send(request);
            return new OkResult();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetTemplateById([FromRoute] int id, [FromQuery] GetTemplateById.Request request)
        {
            request.Id = id;
            var template = await mediator.Send(request);
            return new OkObjectResult(template);
        }

        [HttpGet("{id:int}/context")]
        public async Task<ActionResult> GetTemplateContextById([FromRoute] int id, [FromQuery] GetTemplateById.Request request)
        {
            request.Id = id;
            var template = await mediator.Send(request);
            return new OkObjectResult(template.Context);
        }

        [HttpGet("")]
        public async Task<ActionResult> GetTemplates([FromQuery] GetTemplates.Request request)
        {
            var templates = await mediator.Send(request);
            return new OkObjectResult(templates);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteTemplateById([FromRoute] int id, [FromQuery] DeleteTemplateById.Request request)
        {
            request.Id = id;
            await mediator.Send(request);
            return new OkResult();
        }
    }
}