using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeGenerator.Data.Configs
{
    public class SecurityConfig
    {
        public string SecurityCookieName { get; set; }
        public string CsrfCookieName { get; set; }
        public string CsrfRequestHeaderName { get; set; }
    }
}
