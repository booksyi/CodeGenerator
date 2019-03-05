using System;
using System.Collections.Generic;

namespace CodeGenerator.Data.Models
{
    public partial class Template
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
