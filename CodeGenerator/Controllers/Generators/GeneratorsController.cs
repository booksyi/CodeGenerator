using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGenerator.Handlers;
using CodeGenerator.Handlers.ApiActions;
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

        // /api/Generators/GenerateModel
        [HttpGet("[action]")]
        public async Task<ActionResult> GenerateModel(string connectionString, string tableName, string modelName, string projectName)
        {
            // For test
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                connectionString = @"Server=DESKTOP-RO8TCIK\SQLEXPRESS;Database=TIP_TEST;UID=sa;PWD=1234;";
            }
            if (string.IsNullOrWhiteSpace(tableName))
            {
                tableName = "Article";
            }
            if (string.IsNullOrWhiteSpace(modelName))
            {
                modelName = "Article";
            }


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

            string connectionString = @"Server=DESKTOP-RO8TCIK\SQLEXPRESS;Database=TIP_TEST;UID=sa;PWD=1234;";
            string tableName = "Article";
            string modelName = "Article";

            string projectName = "Unknow";
            DbTableSchema tableSchema = CodingHelper.GetDbTableSchema(connectionString, tableName);

            await System.IO.File.WriteAllTextAsync(
                System.IO.Path.Combine(output, $"Create{modelName}.cs"),
                (await this.mediator.Send(new GenerateCreate.Request()
                {
                    TableSchema = tableSchema,
                    ProjectName = projectName,
                    ModelName = modelName
                })).Generate());

            await System.IO.File.WriteAllTextAsync(
                System.IO.Path.Combine(output, $"Delete{modelName}ById.cs"),
                (await this.mediator.Send(new GenerateDeleteBy.Request()
                {
                    TableSchema = tableSchema,
                    ProjectName = projectName,
                    ModelName = modelName,
                    By = Data.FindBy.Id
                })).Generate());

            await System.IO.File.WriteAllTextAsync(
                System.IO.Path.Combine(output, $"Delete{modelName}ByKey.cs"),
                (await this.mediator.Send(new GenerateDeleteBy.Request()
                {
                    TableSchema = tableSchema,
                    ProjectName = projectName,
                    ModelName = modelName,
                    By = Data.FindBy.Key
                })).Generate());

            await System.IO.File.WriteAllTextAsync(
                System.IO.Path.Combine(output, $"Get{modelName}ById.cs"),
                (await this.mediator.Send(new GenerateGetBy.Request()
                {
                    TableSchema = tableSchema,
                    ProjectName = projectName,
                    ModelName = modelName,
                    By = Data.FindBy.Id
                })).Generate());

            await System.IO.File.WriteAllTextAsync(
                System.IO.Path.Combine(output, $"Get{modelName}ByKey.cs"),
                (await this.mediator.Send(new GenerateGetBy.Request()
                {
                    TableSchema = tableSchema,
                    ProjectName = projectName,
                    ModelName = modelName,
                    By = Data.FindBy.Key
                })).Generate());

            await System.IO.File.WriteAllTextAsync(
                System.IO.Path.Combine(output, $"Edit{modelName}ById.cs"),
                (await this.mediator.Send(new GenerateUpdateBy.Request()
                {
                    TableSchema = tableSchema,
                    ProjectName = projectName,
                    ModelName = modelName,
                    By = Data.FindBy.Id
                })).Generate());

            await System.IO.File.WriteAllTextAsync(
                System.IO.Path.Combine(output, $"Edit{modelName}ByKey.cs"),
                (await this.mediator.Send(new GenerateUpdateBy.Request()
                {
                    TableSchema = tableSchema,
                    ProjectName = projectName,
                    ModelName = modelName,
                    By = Data.FindBy.Key
                })).Generate());

            await System.IO.File.WriteAllTextAsync(
                System.IO.Path.Combine(output, $"Get{pluralizer.Pluralize(modelName)}.cs"),
                (await this.mediator.Send(new GenerateGet.Request()
                {
                    TableSchema = tableSchema,
                    ProjectName = projectName,
                    ModelName = modelName
                })).Generate());

            return Ok();
        }
    }
}