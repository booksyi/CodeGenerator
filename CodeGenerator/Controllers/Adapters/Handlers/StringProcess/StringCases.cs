using HelpersForCore;
using MediatR;
using Pluralize.NET.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.Adapters.Handlers.StringProcess
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
                Func<string, string[]> separate = (sender) =>
                {
                    string engWithNum = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
                    string uppers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    string lowers = "abcdefghijklmnopqrstuvwxyz";
                    List<string> words = new List<string>();
                    string word = "";
                    foreach (char c in sender)
                    {
                        char lastC = word.LastOrDefault();
                        if (engWithNum.Contains(c) == false)
                        {
                            words.Add(word);
                            word = "";
                        }
                        else if (lowers.Contains(lastC) && uppers.Contains(c))
                        {
                            words.Add(word);
                            word = $"{c}";
                        }
                        else
                        {
                            word = $"{word}{c}";
                        }
                    }
                    words.Add(word);
                    return words.Where(x => string.IsNullOrEmpty(x) == false).ToArray();
                };

                string[] nameWords = separate(request.Name);

                string singular = pluralizer.Singularize(request.Name);
                string[] singularWords = separate(singular);

                string plural = pluralizer.Pluralize(request.Name);
                string[] pluralWords = separate(plural);

                return new Response()
                {
                    UpperFirst = request.Name.UpperFirst(),
                    LowerFirst = request.Name.LowerFirst(),
                    LowerDash = string.Join("-", nameWords.Select(x => x.ToLower())),
                    LowerUnderLine = string.Join("_", nameWords.Select(x => x.ToLower())),
                    SingularUpperFirst = singular.UpperFirst(),
                    SingularLowerFirst = singular.LowerFirst(),
                    SingularLowerDash = string.Join("-", singularWords.Select(x => x.ToLower())),
                    SingularLowerUnderLine = string.Join("_", singularWords.Select(x => x.ToLower())),
                    PluralUpperFirst = plural.UpperFirst(),
                    PluralLowerFirst = plural.LowerFirst(),
                    PluralLowerDash = string.Join("-", pluralWords.Select(x => x.ToLower())),
                    PluralLowerUnderLine = string.Join("_", pluralWords.Select(x => x.ToLower())),
                };
            }
        }
    }
}
