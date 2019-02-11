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
                RequestNode node = new RequestNode()
                {
                    From = RequestFrom.Template,
                    TemplateNode = new ApiNode()
                    {
                        Url = $"{host}/api/templates/10/context"
                    },
                    SimpleTemplateRequestNodes = new Dictionary<string, RequestNode>()
                    {
                        {
                            "ProjectName", new RequestNode()
                            {
                                From = RequestFrom.HttpRequest,
                                HttpRequestKey = "ProjectName"
                            }
                        },
                        {
                            "TableName", new RequestNode()
                            {
                                From = RequestFrom.HttpRequest,
                                HttpRequestKey = "TableName",
                                HttpRequestDescription = "資料表"
                            }
                        },
                        {
                            "ModelName", new RequestNode()
                            {
                                From = RequestFrom.HttpRequest,
                                HttpRequestKey = "ModelName"
                            }
                        },
                        {
                            "Properties", new RequestNode()
                            {
                                From = RequestFrom.Template,
                                TemplateNode = new ApiNode()
                                {
                                    Url = $"{host}/api/templates/9/context"
                                },
                                AdapterNodes = new Dictionary<string, AdapterNode>()
                                {
                                    {
                                        "PropertiesAdapter", new AdapterNode()
                                        {
                                            HttpMethod = AdapterHttpMethod.Get,
                                            Url = $"{host}/api/adapters/db/table/fields",
                                            RequestNodes = new Dictionary<string, RequestSimpleNode>()
                                            {
                                                {
                                                    "ConnectionString", new RequestSimpleNode()
                                                    {
                                                        From = RequestSimpleFrom.HttpRequest,
                                                        HttpRequestKey = "ConnectionString"
                                                    }
                                                },
                                                {
                                                    "TableName", new RequestSimpleNode()
                                                    {
                                                        From = RequestSimpleFrom.HttpRequest,
                                                        HttpRequestKey = "TableName",
                                                        HttpRequestDescription = "資料表名稱"
                                                    }
                                                }
                                            },
                                            Type = AdapterType.Separation
                                        }
                                    },
                                },
                                SimpleTemplateRequestNodes = new Dictionary<string, RequestNode>()
                                {
                                    {
                                        "Summary", new RequestNode()
                                        {
                                            From = RequestFrom.Template,
                                            TemplateNode = new ApiNode()
                                            {
                                                Url = $"{host}/api/templates/8/context"
                                            },
                                            SimpleTemplateRequestNodes = new Dictionary<string, RequestNode>()
                                            {
                                                {
                                                    "Text", new RequestNode()
                                                    {
                                                        From = RequestFrom.Adapter,
                                                        AdapterName = "PropertiesAdapter",
                                                        AdapterPropertyName = "Description"
                                                    }
                                                }
                                            }
                                        }
                                    },
                                    {
                                        "Attributes", new RequestNode()
                                        {
                                            From = RequestFrom.Adapter,
                                            AdapterName = "PropertiesAdapter",
                                            AdapterPropertyName = "ForCs.EFAttributes"
                                        }
                                    },
                                    {
                                        "Prefix", new RequestNode()
                                        {
                                            From = RequestFrom.Value,
                                            Value = "public"
                                        }
                                    },
                                    {
                                        "TypeName", new RequestNode()
                                        {
                                            From = RequestFrom.Adapter,
                                            AdapterName = "PropertiesAdapter",
                                            AdapterPropertyName = "ForCs.TypeName"
                                        }
                                    },
                                    {
                                        "PropertyName", new RequestNode()
                                        {
                                            From = RequestFrom.Adapter,
                                            AdapterName = "PropertiesAdapter",
                                            AdapterPropertyName = "Name"
                                        }
                                    },
                                }
                            }
                        }
                    }
                };
                return JsonConvert.SerializeObject(node);
            }
        }
    }
}
