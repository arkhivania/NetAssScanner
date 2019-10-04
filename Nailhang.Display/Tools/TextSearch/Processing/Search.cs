using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nailhang.Display.Tools.TextSearch.Base;

namespace Nailhang.Display.Tools.TextSearch.Processing
{
    class Search : ISearch
    {
        private readonly Index index;

        public Search(Index index)
        {
            this.index = index;
        }

        IEnumerable<Response> ISearch.Search(Request request)
        {
            var cnts = new Dictionary<int, int>();
            var tripletCodes = new HashSet<int>();
            int maxCount = 0;

            foreach (var w in request.Message
                .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(q => q.ToLower()))
            {
                foreach(var t in index.Triplets(w))
                    tripletCodes.Add(t);
            }

            foreach (var t in index.Decode(tripletCodes))
            {
                foreach (var b in t.Bulks)
                {
                    int count = 0;
                    cnts.TryGetValue(b, out count);
                    count++;
                    cnts[b] = count;
                    maxCount = Math.Max(maxCount, count);
                }
            }

            return cnts
                .Select(q => new Response
                {
                    DocumentIndex = q.Key,
                    Relevance = q.Value / (float)maxCount
                }).Where(q => q.Relevance >= request.TargetRelevance);
        }
    }
}
