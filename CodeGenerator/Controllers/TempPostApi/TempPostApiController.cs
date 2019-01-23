using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGenerator.Controllers.TempPostApi.Handlers;
using CodeGenerator.Data.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CodeGenerator.Controllers.TempPostApi
{
    [Route("api/[controller]")]
    public class TempPostApiController : Controller
    {
        private readonly IMediator mediator;

        public TempPostApiController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("")]
        public async Task<ActionResult> Create([FromBody] string result)
        {
            CreateTempPostApi.Request request = new CreateTempPostApi.Request()
            {
                Result = result
            };
            int id = await mediator.Send(request);
            return new OkObjectResult(id);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateById([FromRoute] int id, [FromBody] DbTempPostApi tempPostApi)
        {
            tempPostApi.Id = id;
            UpdateTempPostApiById.Request request = new UpdateTempPostApiById.Request()
            {
                TempPostApi = tempPostApi
            };
            await mediator.Send(request);
            return new OkResult();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetById([FromRoute] int id)
        {
            GetTempPostApiById.Request request = new GetTempPostApiById.Request()
            {
                Id = id
            };
            var tempPostApi = await mediator.Send(request);
            return new OkObjectResult(tempPostApi);
        }

        [HttpPost("{id:int}/post")]
        public async Task<ActionResult> Post([FromRoute] int id)
        {
            GetTempPostApiById.Request request = new GetTempPostApiById.Request()
            {
                Id = id
            };
            var tempPostApi = await mediator.Send(request);
            return Content(tempPostApi.Result);
        }
    }
}