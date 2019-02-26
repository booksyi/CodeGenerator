using CodeGenerator.Data.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.Generators.Handlers
{
    public class GetGenerators
    {
        public class Request : IRequest<Generator[]>
        {
        }

        public class Handler : IRequestHandler<Request, Generator[]>
        {
            private readonly CodeGeneratorContext context;
            public Handler(CodeGeneratorContext context)
            {
                this.context = context;
            }

            public async Task<Generator[]> Handle(Request request, CancellationToken token)
            {
                Generator[] generators = context.Generators.ToArray();
                return generators;
            }
        }
    }
}
