using HelpersForCore;
using MediatR;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.RequestNodes.Handlers
{
    public class GetRequestNodeJsonFromDeveloper
    {
        public class Request : IRequest<string>
        {
        }

        public class Handler : IRequestHandler<Request, string>
        {
            private readonly IHttpContextAccessor httpContextAccessor;
            public Handler(IHttpContextAccessor httpContextAccessor)
            {
                this.httpContextAccessor = httpContextAccessor;
            }

            public async Task<string> Handle(Request request, CancellationToken token)
            {
                string host = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host.Value}";

                CodeTemplate template = new CodeTemplate()
                {
                    Inputs = new CodeTemplate.Input[]
                    {
                        new CodeTemplate.Input("ProjectName").HasDescription("專案名稱"),
                        new CodeTemplate.Input("TableName").HasDescription("資料表名稱"),
                        new CodeTemplate.Input("ModelName").HasDescription("資料模型名稱"),
                        new CodeTemplate.Input("ConnectionString").HasDescription("連接字串"),
                    },
                    TemplateNodes = new CodeTemplate.TemplateNode[]
                    {
                        new CodeTemplate.TemplateNode()
                        {
                            Url = $"{host}/api/templates/10/context",
                            ParameterNodes = new CodeTemplate.ParameterNode[]
                            {
                                new CodeTemplate.ParameterNode("ProjectName").FromInput("ProjectName"),
                                new CodeTemplate.ParameterNode("TableName").FromInput("TableName"),
                                new CodeTemplate.ParameterNode("ModelName").FromInput("ModelName"),
                                new CodeTemplate.ParameterNode("Properties")
                                {
                                    From = CodeTemplate.ParameterFrom.Template,
                                    TemplateNode = new CodeTemplate.TemplateNode()
                                    {
                                        Url = $"{host}/api/templates/9/context",
                                        AdapterNodes = new CodeTemplate.AdapterNode[]
                                        {
                                            new CodeTemplate.AdapterNode(
                                                "PropertiesAdapter",
                                                $"{host}/api/adapters/db/table/fields",
                                                CodeTemplate.HttpMethod.Get)
                                            {
                                                RequestNodes = new CodeTemplate.RequestNode[]
                                                {
                                                    new CodeTemplate.RequestNode("ConnectionString").FromInput("ConnectionString"),
                                                    new CodeTemplate.RequestNode("TableName").FromInput("TableName")
                                                },
                                                IsSplit = true
                                            }
                                        },
                                        ParameterNodes = new CodeTemplate.ParameterNode[]
                                        {
                                            new CodeTemplate.ParameterNode("Summary")
                                            {
                                                From = CodeTemplate.ParameterFrom.Template,
                                                TemplateNode = new CodeTemplate.TemplateNode()
                                                {
                                                    Url = $"{host}/api/templates/8/context",
                                                    ParameterNodes = new CodeTemplate.ParameterNode[]
                                                    {
                                                        new CodeTemplate.ParameterNode("Text").FromAdapter("PropertiesAdapter", "Description")
                                                    }
                                                }
                                            },
                                            new CodeTemplate.ParameterNode("Attributes").FromAdapter("PropertiesAdapter", "ForCs.EFAttributes"),
                                            new CodeTemplate.ParameterNode("Prefix").FromValue("public"),
                                            new CodeTemplate.ParameterNode("TypeName").FromAdapter("PropertiesAdapter", "ForCs.TypeName"),
                                            new CodeTemplate.ParameterNode("PropertyName").FromAdapter("PropertiesAdapter", "Name"),
                                        }
                                    }
                                }
                            }
                        }
                    }
                };

                return JsonConvert.SerializeObject(template);
            }
        }
    }
}
