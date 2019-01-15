using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeGenerator.Data.Models
{
    public class RequestNode
    {
        public RequestType Type { get; set; } = RequestType.Request;
        public string Key { get; set; }
        public Dictionary<string, RequestNode> Parameters { get; set; }

        public Dictionary<string, Adapter> Adapters { get; set; }
        public GenerateMode Mode { get; set; } = GenerateMode.Unification;

        public RequestNode() { }
        public RequestNode(string key) { Key = key; }
    }

    public class Adapter
    {
        public string Url { get; set; }
        public Dictionary<string, RequestNode> Request { get; set; }
        public string[] GroupBy { get; set; }
    }

    public enum RequestType
    {
        /// <summary>
        /// 請求
        /// </summary>
        Request,
        /// <summary>
        /// 樣板
        /// </summary>
        Template
    }

    public enum GenerateMode
    {
        /// <summary>
        /// 統一
        /// </summary>
        Unification,
        /// <summary>
        /// 分離
        /// </summary>
        Separation
    }
}
