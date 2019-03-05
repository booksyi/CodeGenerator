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
        [HttpPut("{id:int}")]
        public async Task<ActionResult> CreateOrUpdateTemplate([FromRoute] int id, [FromBody] CreateOrUpdateTemplate.Request request)
        {
            request.Id = id;
            Template template = await mediator.Send(request);
            return Ok(template);
        }

        [HttpGet("")]
        public async Task<ActionResult> GetTemplates([FromQuery] GetTemplates.Request request)
        {
            var templates = await mediator.Send(request);
            return new OkObjectResult(templates);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetTemplateById([FromRoute] int id)
        {
            GetTemplateById.Request request = new GetTemplateById.Request()
            {
                Id = id
            };
            var template = await mediator.Send(request);
            return new OkObjectResult(template);
        }

        [HttpGet("{id:int}/content")]
        public async Task<ActionResult> GetTemplateContextById([FromRoute] int id)
        {
            GetTemplateById.Request request = new GetTemplateById.Request()
            {
                Id = id
            };
            var template = await mediator.Send(request);
            return Content(template.Content);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteTemplateById([FromRoute] int id)
        {
            DeleteTemplateById.Request request = new DeleteTemplateById.Request()
            {
                Id = id
            };
            await mediator.Send(request);
            return NoContent();
        }
    }
}