using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.Adapters.Actions
{
    public class DbTableInfo
    {
        public class Request : IRequest<IEnumerable<KeyValuePair<string, string>>>
        {
            public string ConnectionString { get; set; }
            public string TableName { get; set; }
        }

        public class Handler : IRequestHandler<Request, IEnumerable<KeyValuePair<string, string>>>
        {
            public Handler()
            {
            }

            public async Task<IEnumerable<KeyValuePair<string, string>>> Handle(Request request, CancellationToken token)
            {
                List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();
                return result;
            }
        }
    }
}
