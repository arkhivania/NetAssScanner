using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nailhang.Display.Tools.TextSearch.Processing
{
    class StringMath
    {
        public static IEnumerable<string> Words(string sentence)
        {
            return sentence
                .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static IEnumerable<string> WordsMakeLower(string sentence)
        {
            return Words(sentence)
                .Select(q => q.ToLower());
        }
    }
}
