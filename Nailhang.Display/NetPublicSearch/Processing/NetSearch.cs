using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Nailhang.Display.NetPublicSearch.Base;
using Nailhang.Display.Tools.TextSearch.Base;
using Nailhang.IndexBase.PublicApi;

namespace Nailhang.Display.NetPublicSearch.Processing
{
    class NetSearch : Base.INetSearch, IDisposable
    {
        struct CurSearch
        {
            public SearchItem[] searchItems;
            public IStat stat;
            public ISearch classSearch;
            public DateTime BuildTime;
        }

        static IEnumerable<Base.SearchItem> Prepare(AssemblyPublic assemblyPublic)
        {
            foreach (var c in assemblyPublic.Classes)
                yield return new SearchItem { Assembly = assemblyPublic, Class = c };
        }

        CurSearch? currentSearch;
        private readonly IWSBuilder wSBuilder;
        private readonly IPublicApiStorage publicApiStorage;



        //readonly ISearch namespaceSearch;

        readonly Timer refreshTimer;

        public NetSearch(IWSBuilder wSBuilder, IPublicApiStorage publicApiStorage)
        {   
            this.wSBuilder = wSBuilder;
            this.publicApiStorage = publicApiStorage;

            refreshTimer = new Timer(state =>
            {
                lock(this)
                {
                    if (currentSearch != null 
                        && DateTime.UtcNow.Subtract(currentSearch.Value.BuildTime) > TimeSpan.FromMinutes(10))
                        currentSearch = null;
                }
            }, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
        }

        private CurSearch CreateSearch()
        {
            var res = new CurSearch();
            res.searchItems = publicApiStorage.LoadAssemblies().SelectMany(a => Prepare(a))
                            .ToArray();

            {
                var search_strings = res.searchItems.Select(w => w.Class.Name);
                res.stat = wSBuilder.BuildStat(search_strings);
                var index = wSBuilder.Index(search_strings.Select(w => new Bulk { Sentence = w }), res.stat);
                res.classSearch = wSBuilder.CreateSearch(index);
            }
            res.BuildTime = DateTime.UtcNow;
            return res;
        }

        public IEnumerable<SearchItem> Search(string query, int maxCount)
        {
            CurSearch search;
            lock (this)
            {
                if (currentSearch == null 
                    || DateTime.UtcNow.Subtract(currentSearch.Value.BuildTime) > TimeSpan.FromMinutes(10))
                    currentSearch = CreateSearch();

                search = currentSearch.Value;
            }

            var name = query.ToLower();

            var relevance = new Relevance(search.stat);

            var filtered = search.classSearch.Search(new Request { Message = query, TargetRelevance = 0.3f })
                .OrderByDescending(w => w.Relevance)
                .Select(q => search.searchItems[q.DocumentIndex])
                .ToArray();

            return relevance
                .ProcessResults(query, filtered)
                .Take(maxCount > 0 ? maxCount : int.MaxValue);
        }

        public void Dispose()
        {
            refreshTimer.Dispose();
        }
    }
}
