using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nailhang.Display.Tools.TextSearch.Processing
{
    class Triplets
    {
        public Triplets(int code)
        {
            Code = code;
        }

        public HashSet<int> Bulks { get; } = new HashSet<int>();
        public int Code { get; }
    }
}
