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

        /// <summary>
        /// вектор при построении отсортированный
        /// </summary>
        public List<int> Bulks { get; } = new List<int>();
        public int Code { get; }
    }
}
