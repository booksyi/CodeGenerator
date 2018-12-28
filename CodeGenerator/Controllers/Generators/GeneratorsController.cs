using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGenerator.Handlers;
using HelpersForCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pluralize.NET.Core;

namespace CodeGenerator.Controllers.Generators
{
    [Route("api/[controller]")]
    public class GeneratorsController : Controller
    {
        private readonly IMediator mediator;
        private readonly Pluralizer pluralizer;

        public GeneratorsController(IMediator mediator, Pluralizer pluralizer)
        {
            this.mediator = mediator;
            this.pluralizer = pluralizer;
        }

        
        [HttpGet("[action]")]
        public async Task<ActionResult> GenerateModel(string connectionString, string tableName, string modelName, string projectName)
        {
            //tableName = "Article";
            //modelName = "Article";

            //connectionString = "Server=LAPTOP-RD9P71LP\\SQLEXPRESS;Database=TEST1;UID=sa;PWD=1234;";
            DbTableSchema tableSchema = CodingHelper.GetDbTableSchema(connectionString, tableName);

            string code = (await mediator.Send(new GenerateModel.Request()
            {
                ProjectName = projectName,
                TableSchema = tableSchema,
                ModelName = modelName
            })).Generate();

            return new OkObjectResult(code);
        }

        // /api/Generators/Test
        [HttpGet("[action]")]
        public async Task<ActionResult> Test()
        {
            string output = @"D:\temp\output";

            await System.IO.File.WriteAllTextAsync(
                System.IO.Path.Combine(output, $"Handler.cs"),
                (await this.mediator.Send(new GenerateClassDependencyInjection.Request()
                {
                    ClassName = "Handler",
                    Fields = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("IMediator", "mediator"),
                        new KeyValuePair<string, string>("Pluralizer", "pluralizer"),
                        new KeyValuePair<string, string>("DatabaseContext", "context")
                    }
                })).Generate());

            return Ok();
        }
    }
}