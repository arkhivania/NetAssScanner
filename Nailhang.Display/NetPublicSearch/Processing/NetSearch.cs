using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nailhang.Display.NetPublicSearch.Base;
using Nailhang.Display.Tools.TextSearch.Base;
using Nailhang.IndexBase.PublicApi;

namespace Nailhang.Display.NetPublicSearch.Processing
{
    class NetSearch : Base.INetSearch
    {
        readonly SearchItem[] searchItems;

        static IEnumerable<Base.SearchItem> Prepare(AssemblyPublic assemblyPublic)
        {
            foreach (var c in assemblyPublic.Classes)
                yield return new SearchItem { Assembly = assemblyPublic, Class = c };
        }

        readonly IStat stat;
        readonly ISearch classSearch;

        //readonly ISearch namespaceSearch;

        public NetSearch(IWSBuilder wSBuilder, IPublicApiStorage publicApiStorage)
        {
            searchItems = publicApiStorage.LoadAssemblies().SelectMany(a => Prepare(a))
                .ToArray();

            {
                var search_strings = searchItems.Select(w => w.Class.Name);
                this.stat = wSBuilder.BuildStat(search_strings);
                var index = wSBuilder.Index(search_strings.Select(w => new Bulk { Sentence = w }), stat);
                this.classSearch = wSBuilder.CreateSearch(index);
            }
        }

        public IEnumerable<SearchItem> Search(string query, int maxCount)
        {
            var name = query.ToLower();

            var relevance = new Relevance(stat);

            var filtered = classSearch.Search(new Request { Message = query, TargetRelevance = 0.3f })
                .OrderByDescending(w => w.Relevance)
                .Select(q => searchItems[q.DocumentIndex])
                .ToArray();

            return relevance.ProcessResults(query, filtered)
                .Take(maxCount > 0 ? maxCount : int.MaxValue);
        }
    }
}
