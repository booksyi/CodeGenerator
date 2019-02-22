using CodeGenerator.Data.Models;
using HelpersForCore;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.Templates.Handlers
{
    public class CreateTemplate
    {
        public class Request : IRequest<DbTemplate>
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Context { get; set; }
        }

        public class Handler : IRequestHandler<Request, DbTemplate>
        {
            private readonly CodeGeneratorContext context;
            public Handler(CodeGeneratorContext context)
            {
                this.context = context;
            }

            public async Task<DbTemplate> Handle(Request request, CancellationToken token)
            {
                DbTemplate template = new DbTemplate()
                {
                    Name = request.Name,
                    Description = request.Description,
                    Context = request.Context
                };
                await context.Templates.AddAsync(template);
                await context.SaveChangesAsync();
                return template;
            }
        }
    }
}
