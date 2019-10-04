using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nailhang.Display.Tools.TextSearch.Processing
{
    class CharStat
    {
        private readonly char @char;
        public char Char => @char;

        public CharStat(char @char)
        {
            this.@char = @char;
        }

        public int Count { get; set; }

        public override string ToString()
        {
            return $"{@char} {Count}";
        }
    }
}
