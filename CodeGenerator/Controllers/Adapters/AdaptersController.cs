using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGenerator.Controllers.Adapters.Handlers;
using CodeGenerator.Controllers.Adapters.Handlers.Database;
using HelpersForCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace CodeGenerator.Controllers.Adapters
{
    [Route("api/[controller]")]
    public class AdaptersController : Controller
    {
        private readonly IMediator mediator;

        public AdaptersController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("db/table")]
        public async Task<ActionResult> DatabaseGetTable([FromBody] GetTable.Request request)
        {
            return new OkObjectResult(await mediator.Send(request));
        }

        [HttpPost("db/table/fields")]
        public async Task<ActionResult> DatabaseGetTableFields([FromBody] GetTableFields.Request request)
        {
            return new OkObjectResult(await mediator.Send(request));
        }

        [HttpPost("string/cases")]
        public async Task<ActionResult> StringCases([FromBody] StringCases.Request request)
        {
            return new OkObjectResult(await mediator.Send(request));
        }
    }
}