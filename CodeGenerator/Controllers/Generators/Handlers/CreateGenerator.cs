using CodeGenerator.Data.Models;
using HelpersForCore;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.Generators.Handlers
{
    public class CreateGenerator
    {
        public class Request : IRequest<Generator>
        {
            public string Name { get; set; }
            public CodeTemplate CodeTemplate { get; set; }
        }

        public class Handler : IRequestHandler<Request, Generator>
        {
            private readonly CodeGeneratorContext context;
            public Handler(CodeGeneratorContext context)
            {
                this.context = context;
            }

            public async Task<Generator> Handle(Request request, CancellationToken token)
            {
                if (request.CodeTemplate.Validate(out string[] errorMessages))
                {
                    Generator generator = new Generator()
                    {
                        Name = request.Name,
                        Json = JsonConvert.SerializeObject(request.CodeTemplate)
                    };
                    await context.Generators.AddAsync(generator);
                    await context.SaveChangesAsync();
                    return generator;
                }
                throw new Exception(string.Join("\r\n", errorMessages));
            }
        }
    }
}
