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
        public async Task<ActionResult> Create([FromBody] DbTemplate template)
        {
            CreateTemplate.Request request = new CreateTemplate.Request()
            {
                Template = template
            };
            int id = await mediator.Send(request);
            return new OkObjectResult(id);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateById([FromRoute] int id, [FromBody] DbTemplate template)
        {
            template.Id = id;
            UpdateTemplateById.Request request = new UpdateTemplateById.Request()
            {
                Template = template
            };
            await mediator.Send(request);
            return new OkResult();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetById([FromRoute] int id)
        {
            GetTemplateById.Request request = new GetTemplateById.Request()
            {
                Id = id
            };
            var template = await mediator.Send(request);
            return new OkObjectResult(template);
        }

        [HttpGet("{id:int}/context")]
        public async Task<ActionResult> GetContextById([FromRoute] int id)
        {
            GetTemplateById.Request request = new GetTemplateById.Request()
            {
                Id = id
            };
            var template = await mediator.Send(request);
            return new OkObjectResult(template.Context);
        }

        [HttpGet("")]
        public async Task<ActionResult> Get()
        {
            GetTemplates.Request request = new GetTemplates.Request();
            var templates = await mediator.Send(request);
            return new OkObjectResult(templates);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteById([FromRoute] int id)
        {
            DeleteTemplateById.Request request = new DeleteTemplateById.Request()
            {
                Id = id
            };
            await mediator.Send(request);
            return new OkResult();
        }
    }
}