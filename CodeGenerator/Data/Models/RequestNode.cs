using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeGenerator.Data.Models
{
    public class RequestNode
    {
        public Dictionary<string, object> Request { get; set; }
        public Dictionary<string, AdapterNode> AdapterNodes { get; set; } = new Dictionary<string, AdapterNode>();
        public Dictionary<string, Dictionary<string, object>> Adapters { get; set; } = new Dictionary<string, Dictionary<string, object>>();
        public RequestFrom From { get; set; } = RequestFrom.Request;
        public string AdapterName { get; set; }
        public string Key { get; set; }
        public Dictionary<string, RequestNode> Parameters { get; set; }

        public RequestNode() { }
        public RequestNode(string key)
        {
            From = RequestFrom.Request;
            Key = key;
        }
        public RequestNode(string adapterName, string key)
        {
            From = RequestFrom.Adapter;
            AdapterName = adapterName;
            Key = key;
        }
    }

    public class AdapterNode
    {
        public string Url { get; set; }
        public Dictionary<string, RequestNode> Request { get; set; }
        public AdapterType Type { get; set; }
    }

    public enum RequestFrom
    {
        /// <summary>
        /// 請求
        /// </summary>
        Request,
        /// <summary>
        /// 中繼資料
        /// </summary>
        Adapter,
        /// <summary>
        /// 樣板
        /// </summary>
        Template
    }

    public enum AdapterType
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
