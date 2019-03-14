using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeGenerator.Data.Configs
{
    public class JwtConfig
    {
        public int MinutesUntilExpiry { get; set; }
        public string SigningSecret { get; set; }
    }
}
