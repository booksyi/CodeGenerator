using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGenerator.Controllers.Generators.Actions;
using HelpersForCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CodeGenerator.Controllers.Generators
{
    [Route("api/[controller]")]
    public class GeneratorsController : Controller
    {
        private readonly IMediator mediator;

        public GeneratorsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult> GenerateModel([FromQuery] GenerateModel.Request request)
        {
            var response = await this.mediator.Send(request);
            return response.Ok();
        }

        // /api/Generators/Test
        [HttpGet("[action]")]
        public async Task<ActionResult> Test()
        {
            string projectName = "CodeGenerator";
            string tableName = "Abcde";
            string modelName = "Abcde";
            string modelOutputPath = @"D:\Workspace\CodeGenerator\CodeGenerator\Models";
            string controllerOutputPath = @"D:\Workspace\CodeGenerator\CodeGenerator\Controllers\Abcdes";
            string actionOutputPath = @"D:\Workspace\CodeGenerator\CodeGenerator\Controllers\Abcdes\Actions";

            await this.mediator.Send(new GenerateModel.Request()
            {
                ProjectName = projectName,
                TableName = tableName,
                ModelName = modelName,
                OutputPath = modelOutputPath
            });
            await this.mediator.Send(new GenerateController.Request()
            {
                ProjectName = projectName,
                TableName = tableName,
                ModelName = modelName,
                OutputPath = controllerOutputPath
            });
            await this.mediator.Send(new GenerateActionCreate.Request()
            {
                ProjectName = projectName,
                TableName = tableName,
                ModelName = modelName,
                OutputPath = actionOutputPath
            });
            await this.mediator.Send(new GenerateActionRead.Request()
            {
                ProjectName = projectName,
                TableName = tableName,
                ModelName = modelName,
                OutputPath = actionOutputPath
            });
            await this.mediator.Send(new GenerateActionReads.Request()
            {
                ProjectName = projectName,
                TableName = tableName,
                ModelName = modelName,
                OutputPath = actionOutputPath
            });
            await this.mediator.Send(new GenerateActionUpdate.Request()
            {
                ProjectName = projectName,
                TableName = tableName,
                ModelName = modelName,
                OutputPath = actionOutputPath
            });
            await this.mediator.Send(new GenerateActionDelete.Request()
            {
                ProjectName = projectName,
                TableName = tableName,
                ModelName = modelName,
                OutputPath = actionOutputPath
            });

            return Ok();
        }
    }
}