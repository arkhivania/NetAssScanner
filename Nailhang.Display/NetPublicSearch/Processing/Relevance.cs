﻿using Nailhang.Display.Tools.TextSearch.Base;
using Nailhang.IndexBase.PublicApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nailhang.Display.NetPublicSearch.Processing
{
    class Relevance
    {
        private readonly IStat stat;

        public Relevance(IStat stat)
        {
            this.stat = stat;
        }

        class RelItem
        {
            public Base.ISearchItem Item { get; set; }
            public double Relevance;
        }

        public IEnumerable<Base.ISearchItem> ProcessResults(string query, IEnumerable<Base.ISearchItem> searchItems)
        {
            var source = searchItems
                .Select(w => new RelItem { Item = w, Relevance = 0 })
                .ToArray();

            foreach (var ri in source)
            {
                var classNameRelevance = stat.CompareStrings(ri.Item.ClassName, query);
                var namespaceRelevance = stat.CompareStrings(ri.Item.Namespace ?? "", query);
                //double methodRelevance = 0.0;
                //foreach (var m in ri.Item.Class.Methods)
                //{
                //    if (!string.IsNullOrEmpty(m.Name))
                //    {
                //        var cs = stat.CompareStrings(m.Name, query);
                //        if (cs > 0)
                //            methodRelevance += cs;
                //    }
                //}

                ri.Relevance = 2.0 * classNameRelevance + 1 * namespaceRelevance;
                //if (ri.Item.Class.Methods.Length > 0)
                //    ri.Relevance += methodRelevance / ri.Item.Class.Methods.Length;
            }

            return source
                .OrderByDescending(q => q.Relevance)
                .Select(w => w.Item);
        }
    }
}
