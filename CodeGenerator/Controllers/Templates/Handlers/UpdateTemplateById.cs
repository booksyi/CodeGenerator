using CodeGenerator.Data.Models;
using HelpersForCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.Templates.Handlers
{
    public class UpdateTemplateById
    {
        public class Request : IRequest<DbTemplate>
        {
            internal int Id { get; set; }
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
                DbTemplate template = await context.Templates.FirstOrDefaultAsync(x => x.Id == request.Id);
                template.Name = request.Name;
                template.Description = request.Description;
                template.Context = request.Context;
                await context.SaveChangesAsync();
                return template;
            }
        }
    }
}
