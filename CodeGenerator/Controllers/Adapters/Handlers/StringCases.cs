using HelpersForCore;
using MediatR;
using Pluralize.NET.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.Adapters.Handlers
{
    public class StringCases
    {
        public class Request : IRequest<Response>
        {
            public string Name { get; set; }
        }

        public class Response
        {
            public string UpperFirst { get; set; }
            public string LowerFirst { get; set; }
            public string LowerDash { get; set; }
            public string LowerUnderLine { get; set; }

            public string SingularUpperFirst { get; set; }
            public string SingularLowerFirst { get; set; }
            public string SingularLowerDash { get; set; }
            public string SingularLowerUnderLine { get; set; }

            public string PluralUpperFirst { get; set; }
            public string PluralLowerFirst { get; set; }
            public string PluralLowerDash { get; set; }
            public string PluralLowerUnderLine { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly Pluralizer pluralizer;
            public Handler(Pluralizer pluralizer)
            {
                this.pluralizer = pluralizer;
            }

            public async Task<Response> Handle(Request request, CancellationToken token)
            {
                string singular = pluralizer.Singularize(request.Name);
                string plural = pluralizer.Pluralize(request.Name);

                return new Response()
                {
                    UpperFirst = request.Name.UpperFirst(),
                    LowerFirst = request.Name.LowerFirst(),
                    LowerDash = "",
                    LowerUnderLine = "",
                    SingularUpperFirst = singular.UpperFirst(),
                    SingularLowerFirst = singular.LowerFirst(),
                    SingularLowerDash = "",
                    SingularLowerUnderLine = "",
                    PluralUpperFirst = plural.UpperFirst(),
                    PluralLowerFirst = plural.LowerFirst(),
                    PluralLowerDash = "",
                    PluralLowerUnderLine = "",
                };
            }
        }
    }
}
