using CodeGenerator.Data;
using CodeGenerator.Data.Models;
using HelpersForCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.Generators.Handlers
{
    public class CreateOrUpdateGenerator
    {
        public class Request : IRequest<Generator>
        {
            internal int Id { get; set; }
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
                Generator generator;
                if (request.Id == 0)
                {
                    if (request.CodeTemplate.Validate(out string[] errorMessages) == false)
                    {
                        throw new Exception(string.Join("\r\n", errorMessages));
                    }
                    generator = new Generator()
                    {
                        Name = request.Name,
                        Json = JsonConvert.SerializeObject(request.CodeTemplate)
                    };
                    await context.Generators.AddAsync(generator);
                }
                else
                {
                    generator = await context.Generators.FirstOrDefaultAsync(x => x.Id == request.Id);
                    generator.Name = request.Name;
                    generator.Json = JsonConvert.SerializeObject(request.CodeTemplate);
                }
                await context.SaveChangesAsync();
                return generator;
            }
        }
    }
}
