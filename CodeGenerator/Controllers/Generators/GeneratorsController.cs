using System;
using System.Collections.Generic;
using System.IO;
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
            string code = await (await mediator.Send(new GenerateModel.Request()
            {
                ProjectName = projectName,
                TableSchema = tableSchema,
                ModelName = modelName
            })).GenerateAsync();

            return new OkObjectResult(code);
        }

        // /api/Generators/Test
        [HttpGet("[action]")]
        public async Task<ActionResult> GenerateCsApi(
            string projectName,
            string connectionString,
            string[] tableNames)
        {
            List<GeneratorsResource> resources = new List<GeneratorsResource>();

            IEnumerable<DbTableSchema> tableSchemas = CodingHelper.GetDbTableSchema(connectionString, tableNames);
            foreach (DbTableSchema tableSchema in tableSchemas)
            {
                bool useIdentityAsPrimaryKey =
                    tableSchema.PrimaryKeys.Count() == 1
                    && tableSchema.PrimaryKeys.First().Name == tableSchema.Identity.Name;

                #region Model
                resources.Add(await new GeneratorsResource($"{tableSchema.ForCs.ModelName}.cs").BuildTree(await this.mediator.Send(new GenerateModel.Request()
                {
                    TableSchema = tableSchema,
                    ProjectName = projectName,
                    ModelName = tableSchema.ForCs.ModelName
                })));
                #endregion

                #region Controller
                resources.Add(await new GeneratorsResource($"{pluralizer.Pluralize(tableSchema.ForCs.ModelName)}Controller.cs").BuildTree(await this.mediator.Send(new GenerateApiController.Request()
                {
                    TableSchema = tableSchema,
                    ProjectName = projectName
                })));
                #endregion

                #region Actions
                resources.Add(await new GeneratorsResource($"Create{tableSchema.ForCs.ModelName}.cs").BuildTree(await this.mediator.Send(new GenerateCreate.Request()
                {
                    TableSchema = tableSchema,
                    ProjectName = projectName
                })));

                resources.Add(await new GeneratorsResource($"Delete{tableSchema.ForCs.ModelName}ById.cs").BuildTree(await this.mediator.Send(new GenerateDeleteBy.Request()
                {
                    TableSchema = tableSchema,
                    ProjectName = projectName,
                    By = Data.FindBy.Id
                })));

                if (useIdentityAsPrimaryKey == false)
                {
                    resources.Add(await new GeneratorsResource($"Delete{tableSchema.ForCs.ModelName}ByKey.cs").BuildTree(await this.mediator.Send(new GenerateDeleteBy.Request()
                    {
                        TableSchema = tableSchema,
                        ProjectName = projectName,
                        By = Data.FindBy.Key
                    })));
                }

                resources.Add(await new GeneratorsResource($"Get{tableSchema.ForCs.ModelName}ById.cs").BuildTree(await this.mediator.Send(new GenerateGetBy.Request()
                {
                    TableSchema = tableSchema,
                    ProjectName = projectName,
                    By = Data.FindBy.Id
                })));

                if (useIdentityAsPrimaryKey == false)
                {
                    resources.Add(await new GeneratorsResource($"Get{tableSchema.ForCs.ModelName}ByKey.cs").BuildTree(await this.mediator.Send(new GenerateGetBy.Request()
                    {
                        TableSchema = tableSchema,
                        ProjectName = projectName,
                        By = Data.FindBy.Key
                    })));
                }

                resources.Add(await new GeneratorsResource($"Edit{tableSchema.ForCs.ModelName}ById.cs").BuildTree(await this.mediator.Send(new GenerateUpdateBy.Request()
                {
                    TableSchema = tableSchema,
                    ProjectName = projectName,
                    By = Data.FindBy.Id
                })));

                if (useIdentityAsPrimaryKey == false)
                {
                    resources.Add(await new GeneratorsResource($"Edit{tableSchema.ForCs.ModelName}ByKey.cs").BuildTree(await this.mediator.Send(new GenerateUpdateBy.Request()
                    {
                        TableSchema = tableSchema,
                        ProjectName = projectName,
                        By = Data.FindBy.Key
                    })));
                }

                resources.Add(await new GeneratorsResource($"Get{pluralizer.Pluralize(tableSchema.ForCs.ModelName)}.cs").BuildTree(await this.mediator.Send(new GenerateGet.Request()
                {
                    TableSchema = tableSchema,
                    ProjectName = projectName
                })));
                #endregion
            }

            return new OkObjectResult(resources);
        }

        // /api/Generators/Test
        [HttpGet("[action]")]
        public async Task<ActionResult> Test()
        {
            string projectName = "CoreWebFuntions";
            string connectionString = @"Server=LAPTOP-RD9P71LP\SQLEXPRESS;Database=TIP_TEST;UID=sa;PWD=1234;";
            string[] tableNames = new string[] { "Article", "Epaper", "Member" };

            IEnumerable<DbTableSchema> tableSchemas = CodingHelper.GetDbTableSchema(connectionString, tableNames);

            foreach (DbTableSchema tableSchema in tableSchemas)
            {
                string outputController = $@"D:\Workspace\{projectName}\{projectName}\Controllers\{pluralizer.Pluralize(tableSchema.ForCs.ModelName)}";
                string outputActions = $@"D:\Workspace\{projectName}\{projectName}\Controllers\{pluralizer.Pluralize(tableSchema.ForCs.ModelName)}\Actions";
                string outputModels = $@"D:\Workspace\{projectName}\{projectName}\Data\Models";

                if (Directory.Exists(outputController) == false)
                {
                    Directory.CreateDirectory(outputController);
                }
                if (Directory.Exists(outputActions) == false)
                {
                    Directory.CreateDirectory(outputActions);
                }
                if (Directory.Exists(outputModels) == false)
                {
                    Directory.CreateDirectory(outputModels);
                }

                bool useIdentityAsPrimaryKey =
                    tableSchema.PrimaryKeys.Count() == 1
                    && tableSchema.PrimaryKeys.First().Name == tableSchema.Identity.Name;

                #region Model
                await System.IO.File.WriteAllTextAsync(
                    Path.Combine(outputModels, $"{tableSchema.ForCs.ModelName}.cs"),
                    await (await this.mediator.Send(new GenerateModel.Request()
                    {
                        TableSchema = tableSchema,
                        ProjectName = projectName,
                        ModelName = tableSchema.ForCs.ModelName
                    })).GenerateAsync());
                #endregion

                #region Controller
                await System.IO.File.WriteAllTextAsync(
                    Path.Combine(outputController, $"{pluralizer.Pluralize(tableSchema.ForCs.ModelName)}Controller.cs"),
                    await (await this.mediator.Send(new GenerateApiController.Request()
                    {
                        TableSchema = tableSchema,
                        ProjectName = projectName
                    })).GenerateAsync());

                #endregion

                #region Actions
                await System.IO.File.WriteAllTextAsync(
                    Path.Combine(outputActions, $"Create{tableSchema.ForCs.ModelName}.cs"),
                    await (await this.mediator.Send(new GenerateCreate.Request()
                    {
                        TableSchema = tableSchema,
                        ProjectName = projectName
                    })).GenerateAsync());

                await System.IO.File.WriteAllTextAsync(
                    Path.Combine(outputActions, $"Delete{tableSchema.ForCs.ModelName}ById.cs"),
                    await (await this.mediator.Send(new GenerateDeleteBy.Request()
                    {
                        TableSchema = tableSchema,
                        ProjectName = projectName,
                        By = Data.FindBy.Id
                    })).GenerateAsync());

                if (useIdentityAsPrimaryKey == false)
                {
                    await System.IO.File.WriteAllTextAsync(
                        Path.Combine(outputActions, $"Delete{tableSchema.ForCs.ModelName}ByKey.cs"),
                        await (await this.mediator.Send(new GenerateDeleteBy.Request()
                        {
                            TableSchema = tableSchema,
                            ProjectName = projectName,
                            By = Data.FindBy.Key
                        })).GenerateAsync());
                }

                await System.IO.File.WriteAllTextAsync(
                    Path.Combine(outputActions, $"Get{tableSchema.ForCs.ModelName}ById.cs"),
                    await (await this.mediator.Send(new GenerateGetBy.Request()
                    {
                        TableSchema = tableSchema,
                        ProjectName = projectName,
                        By = Data.FindBy.Id
                    })).GenerateAsync());

                if (useIdentityAsPrimaryKey == false)
                {
                    await System.IO.File.WriteAllTextAsync(
                        Path.Combine(outputActions, $"Get{tableSchema.ForCs.ModelName}ByKey.cs"),
                        await (await this.mediator.Send(new GenerateGetBy.Request()
                        {
                            TableSchema = tableSchema,
                            ProjectName = projectName,
                            By = Data.FindBy.Key
                        })).GenerateAsync());
                }


                await System.IO.File.WriteAllTextAsync(
                    Path.Combine(outputActions, $"Edit{tableSchema.ForCs.ModelName}ById.cs"),
                    await (await this.mediator.Send(new GenerateUpdateBy.Request()
                    {
                        TableSchema = tableSchema,
                        ProjectName = projectName,
                        By = Data.FindBy.Id
                    })).GenerateAsync());

                if (useIdentityAsPrimaryKey == false)
                {
                    await System.IO.File.WriteAllTextAsync(
                        Path.Combine(outputActions, $"Edit{tableSchema.ForCs.ModelName}ByKey.cs"),
                        await (await this.mediator.Send(new GenerateUpdateBy.Request()
                        {
                            TableSchema = tableSchema,
                            ProjectName = projectName,
                            By = Data.FindBy.Key
                        })).GenerateAsync());
                }

                await System.IO.File.WriteAllTextAsync(
                    Path.Combine(outputActions, $"Get{pluralizer.Pluralize(tableSchema.ForCs.ModelName)}.cs"),
                    await (await this.mediator.Send(new GenerateGet.Request()
                    {
                        TableSchema = tableSchema,
                        ProjectName = projectName
                    })).GenerateAsync());
                #endregion
            }
            return Ok();
        }
    }
}