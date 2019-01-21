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
        public string SavePath { get; set; }
        public JToken Tree { get; private set; }

        public GeneratorsResource(string savePath)
        {
            SavePath = savePath;
        }

        public async Task<GeneratorsResource> BuildTree(GenerateNode node)
        {
            Tree = await node.ToJTokenAsync();
            return this;
        }
    }
}
