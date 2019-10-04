using Nailhang.Display.Tools.TextSearch.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nailhang.Display.Tools.TextSearch.Processing
{
    class Index : IIndex
    {
        private readonly Statistics statistics;

        readonly Dictionary<int, Triplets> triplets = new Dictionary<int, Triplets>();

        int documentIndex = 0;

        public Index(Statistics statistics)
        {
            this.statistics = statistics;
        }

        public IEnumerable<Triplets> Decode(IEnumerable<int> codes)
        {
            foreach (var c in codes)
            {
                Processing.Triplets tr;
                if (triplets.TryGetValue(c, out tr))
                    yield return tr;
            }
        }

        internal byte ToCode(char c)
        {
            byte bc;
            if (!statistics.ToCode.TryGetValue(c, out bc))
                bc = 255;
            return bc;
        }

        public IEnumerable<int> Triplets(string word)
        {
            var letterCodes = new List<byte>(new byte[] { 255, 255 });
            letterCodes.AddRange(word.Where(char.IsLetter).Select(ToCode));
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

        public IEnumerable<string> Words(string sentence)
        {
            return StringMath.Words(sentence)
                .Select(q => q.ToLower());
        }

        public void Run(Bulk bulk)
        {
            foreach (var w in Words(bulk.Sentence))
            {
                foreach (var code in Triplets(w))
                {
                    Triplets tr;
                    if (!triplets.TryGetValue(code, out tr))
                        triplets[code] = tr = new Triplets(code);
                    tr.Bulks.Add(documentIndex);
                }
            }

            if (documentIndex == int.MaxValue)
                throw new InvalidOperationException("Index is too large");

            documentIndex++;
        }
    }
}
