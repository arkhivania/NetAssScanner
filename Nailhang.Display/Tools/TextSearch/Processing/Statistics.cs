using Nailhang.Display.Tools.TextSearch.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nailhang.Display.Tools.TextSearch.Processing
{
    class Statistics : IStat
    {
        public CharStat[] Codes { get; set; }
        public Dictionary<char, byte> ToCode { get; } = new Dictionary<char, byte>();
        public HashSet<char> Coded { get; } = new HashSet<char>();

        public Dictionary<char, CharStat> CharStats { get; } = new Dictionary<char, CharStat>();
    }
}
