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
    public class GetTemplates
    {
        public class Request : IRequest<DbTemplate[]>
        {
        }

        public class Handler : IRequestHandler<Request, DbTemplate[]>
        {
            private readonly CodeGeneratorContext context;
            public Handler(CodeGeneratorContext context)
            {
                this.context = context;
            }

            public async Task<DbTemplate[]> Handle(Request request, CancellationToken token)
            {
                DbTemplate[] templates = context.Templates.ToArray();
                return templates;
            }
        }
    }
}
