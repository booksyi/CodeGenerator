using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGenerator.Controllers.Adapters.Handlers.AnalysisCSharp;
using CodeGenerator.Controllers.Adapters.Handlers.Converters;
using CodeGenerator.Controllers.Adapters.Handlers.Database;
using CodeGenerator.Controllers.Adapters.Handlers.StringProcess;
using HelpersForCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("db/table")]
        public async Task<ActionResult> DatabaseGetTable([FromQuery] GetTable.Request request)
        {
            return Ok(await mediator.Send(request));
        }

        [HttpGet("db/table/fields")]
        public async Task<ActionResult> DatabaseGetTableFields([FromQuery] GetTableFields.Request request)
        {
            return Ok(await mediator.Send(request));
        }

        [HttpGet("string/cases")]
        public async Task<ActionResult> StringCases([FromQuery] StringCases.Request request)
        {
            return Ok(await mediator.Send(request));
        }

        [HttpGet("string/format")]
        public async Task<ActionResult> StringFormat([FromQuery] StringFormat.Request request)
        {
            return Ok(await mediator.Send(request));
        }

        [HttpPost("analysis/cs/class")]
        public async Task<ActionResult> AnalysisClass([FromBody] AnalysisClass.Request request)
        {
            return Ok(await mediator.Send(request));
        }

        [HttpPost("converters/csClassToTsClass")]
        public async Task<ActionResult> CsClassToTsClass([FromBody] CsClassToTsClass.Request request)
        {
            return Ok(await mediator.Send(request));
        }
    }
}