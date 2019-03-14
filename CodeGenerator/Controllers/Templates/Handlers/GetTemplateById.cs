using CodeGenerator.Data;
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
    public class GetTemplateById
    {
        public class Request : IRequest<Template>
        {
            internal int Id { get; set; }
        }

        public class Handler : IRequestHandler<Request, Template>
        {
            private readonly CodeGeneratorContext context;
            public Handler(CodeGeneratorContext context)
            {
                this.context = context;
            }

            public async Task<Template> Handle(Request request, CancellationToken token)
            {
                Template template = await context.Templates.FirstOrDefaultAsync(x => x.Id == request.Id);
                return template;
            }
        }
    }
}
