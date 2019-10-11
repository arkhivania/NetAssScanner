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

        internal byte ToCodeFunc(char c)
        {
            byte bc;
            if (!ToCode.TryGetValue(c, out bc))
                bc = 255;
            return bc;
        }

        public IEnumerable<int> Triplets(string word)
        {
            var letterCodes = new List<byte>(new byte[] { 255, 255 });
            letterCodes.AddRange(word.Where(char.IsLetter).Select(ToCodeFunc));
            letterCodes.AddRange(new byte[] { 255, 255 });

            for (int j = 0; j <= letterCodes.Count - 3; ++j)
            {
                int code = 0;
                for (int x = 0; x < 3; ++x)
                {
                    int lc = letterCodes[j + x];
                    code |= lc << x * 8;
                }

                yield return code;
            }
        }

        Dictionary<int, int> StringTripletsStat(string a)
        {
            var res = new Dictionary<int, int>();
            foreach (var w in a
                .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(q => q.ToLower()))
                foreach (var t in Triplets(w))
                {
                    int cnt;
                    res.TryGetValue(t, out cnt);
                    res[t] = cnt + 1;
                }
            return res;
        }

        public double CompareStrings(string a, string b)
        {
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b))
                return 0;

            var tripletsA = StringTripletsStat(a);
            var tripletsB = StringTripletsStat(b);

            double scoreA = 0.0;
            
            foreach (var ta in tripletsA)
            {
                int cntB;
                if (tripletsB.TryGetValue(ta.Key, out cntB))
                    scoreA += 1;
            }

            double scoreB = 0.0;

            foreach (var tb in tripletsB)
            {
                int cntA;
                if (tripletsA.TryGetValue(tb.Key, out cntA))
                    scoreB += 1;
            }

            return scoreA/tripletsA.Count + scoreB/tripletsB.Count;
        }
    }
}
