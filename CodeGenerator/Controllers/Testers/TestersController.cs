using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGenerator.Controllers.Testers.Handlers;
using HelpersForCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace CodeGenerator.Controllers.Testers
{
    [Route("api/[controller]")]
    public class TestersController : Controller
    {
        private readonly IMediator mediator;

        public TestersController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("")]
        public async Task<ActionResult> Test()
        {
            var response = await mediator.Send(new Test.Request());
            return new OkObjectResult(response.Result);
        }

        [HttpGet("templates/{tester}/{template}")]
        public async Task<ActionResult> GetTestTemplate([FromRoute] string tester, [FromRoute] string template)
        {
            GetTestTemplate.Request request = new GetTestTemplate.Request()
            {
                Tester = tester,
                Template = template,
                Query = Request.Query.ToJObject()
            };
            string txt = await mediator.Send(request);
            return Content(txt);
        }

        [HttpGet("adapters/{tester}/{adapter}")]
        [HttpPost("adapters/{tester}/{adapter}")]
        public async Task<ActionResult> GetTestAdapter([FromRoute] string tester, [FromRoute] string adapter)
        {
            GetTestAdapter.Request request = new GetTestAdapter.Request()
            {
                Tester = tester,
                Adapter = adapter
            };
            if (Request.Method == "GET")
            {
                request.Query = Request.Query.ToJObject();
            }
            else if (Request.Method.In("POST", "PUT"))
            {
                request.Query = await Request.Body.ToJObjectAsync();
            }
            JToken result = await mediator.Send(request);
            return new OkObjectResult(result);
        }
    }
}