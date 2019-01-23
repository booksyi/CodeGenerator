using HelpersForCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers.Generators
{
    public class GeneratorsResource
    {
        public string Path { get; set; }
        public string Text { get; set; }

        public GeneratorsResource(string path, string text)
        {
            Path = path;
            Text = text;
        }
    }
}
