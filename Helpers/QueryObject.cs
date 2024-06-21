using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Helpers
{
    public class QueryObject
    {
        public int ? ImageId { get; set; } = null;
        public bool IsDecsending { get; internal set; }
        public string? SortBy { get; internal set; }
    }
}