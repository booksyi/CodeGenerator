using HelpersForCore;
using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Handlers
{
    public class GenerateClassDependencyInjection
    {
        public class Request : IRequest<GenerateNode>
        {
            public List<KeyValuePair<string, string>> Fields { get; set; }
            public string ClassName { get; set; }
        }

        public class Handler : IRequestHandler<Request, GenerateNode>
        {
            private readonly IMediator mediator;

            public Handler(IMediator mediator)
            {
                this.mediator = mediator;
            }

            public async Task<GenerateNode> Handle(Request request, CancellationToken token)
            {
                GenerateNode node = new GenerateNode() { ApplyFilePath = @"Templates\CSharp\DependencyInjection.html" };

                GenerateNode constructorNode = node.AppendChild(await mediator.Send(
                    new GenerateConstructor.Request()
                    {
                        TypeName = request.ClassName,
                        Parameters = null,
                        InnerCodes = null
                    })).ChangeKey("Constructor");

                foreach (var field in request.Fields)
                {
                    constructorNode.AppendChild(await mediator.Send(
                        new GenerateMethodParameter.Request()
                        {
                            TypeName = field.Key,
                            ObjectName = field.Value,
                        })).ChangeKey("Parameters");

                    constructorNode.AppendChild(await mediator.Send(
                        new GenerateConstructorSetValue.Request()
                        {
                            Accept = field.Value,
                            Deliver = field.Value,
                        })).ChangeKey("InnerCodes");

                    node.AppendChild(await mediator.Send(
                        new GenerateDeclareField.Request()
                        {
                            Prefix = "private readonly",
                            TypeName = field.Key,
                            ObjectName = field.Value,
                        })).ChangeKey("Declares");
                }

                return node;
            }
        }
    }
}
