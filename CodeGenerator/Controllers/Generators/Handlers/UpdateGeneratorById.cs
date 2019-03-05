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
    public class UpdateGeneratorById
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
                Generator generator = await context.Generators.FirstOrDefaultAsync(x => x.Id == request.Id);
                generator.Name = request.Name;
                generator.Json = JsonConvert.SerializeObject(request.CodeTemplate);
                await context.SaveChangesAsync();
                return generator;
            }
        }
    }
}
