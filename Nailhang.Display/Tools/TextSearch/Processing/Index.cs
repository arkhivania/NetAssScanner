﻿using Nailhang.Display.Tools.TextSearch.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nailhang.Display.Tools.TextSearch.Processing
{
    class Index : IIndex
    {
        private readonly Statistics statistics;

        internal readonly Dictionary<int, Triplets> triplets = new Dictionary<int, Triplets>();

        int documentIndex = 0;

        internal Statistics Statistics => statistics;

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
        

        public IEnumerable<string> Words(string sentence)
        {
            return StringMath.Words(sentence)
                .Select(q => q.ToLower());
        }

        public void Run(Bulk bulk)
        {
            foreach (var w in Words(bulk.Sentence))
            {
                foreach (var code in statistics.Triplets(w))
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

        public void Throttle(float quantile)
        {
            if (triplets.Count == 0)
                return;

            var triplets_stat = triplets.Select(w => w.Value.Bulks.Count).ToList();
            triplets_stat.Sort();
            var threshold = triplets_stat[(int)((float)triplets_stat.Count - quantile * (float)triplets_stat.Count)];
            int removed = 0;
            foreach(var tv in triplets.ToArray())
                if(tv.Value.Bulks.Count > threshold)
                {
                    triplets.Remove(tv.Key);
                    removed++;
                }

            System.Diagnostics.Trace.WriteLine($"Throttle removed: {removed}");
        }
    }
}
