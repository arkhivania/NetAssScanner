using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nailhang.Web.Models
{
    public struct ChangesRow
    {
        public MonthPeriod Month { get; set; }
        public string[] Users { get; set; }
        public int Count { get; set; }
    }
}
