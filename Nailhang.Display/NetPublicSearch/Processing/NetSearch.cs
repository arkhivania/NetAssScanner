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
            public Base.NamespaceInfo[] Namespaces { get; set; }
        }

        static IEnumerable<Base.SearchItem> Prepare(AssemblyPublic assemblyPublic, HashSet<string> namespaces)
        {
            foreach (var c in assemblyPublic.Classes)
            {
                namespaces.Add(c.Namespace);
                yield return new SearchItem { Assembly = assemblyPublic, Class = c };
            }
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
                lock (this)
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

            var loadedAssemblies = publicApiStorage
                .LoadAssemblies();

            var namespaces = new HashSet<string>();
            res.searchItems = loadedAssemblies
                .SelectMany(a => Prepare(a, namespaces))
                .ToArray();

            {
                var search_strings = res.searchItems.Select(w => w.Class.Name);
                res.stat = wSBuilder.BuildStat(search_strings);
                var index = wSBuilder.Index(search_strings.Select(w => new Bulk { Sentence = w }), res.stat);
                index.Throttle(0.1f);
                res.classSearch = wSBuilder.CreateSearch(index);
            }
            res.BuildTime = DateTime.UtcNow;
            res.Namespaces = namespaces
                .OrderBy(w => w)
                .Select(w => new Base.NamespaceInfo { Namespace = w, Levels = w.Split('.').Length + 1 })
                .ToArray();
            return res;
        }

        public IEnumerable<SearchItem> Search(string query, int maxCount)
        {
            CurSearch search = GetSearch();
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

        private CurSearch GetSearch()
        {
            CurSearch search;
            lock (this)
            {
                if (currentSearch == null
                    || DateTime.UtcNow.Subtract(currentSearch.Value.BuildTime) > TimeSpan.FromMinutes(10))
                {
                    currentSearch = null;
                    currentSearch = CreateSearch();
                }

                search = currentSearch.Value;
            }

            return search;
        }

        public void Dispose()
        {
            refreshTimer.Dispose();
        }

        public void RebuildIndex()
        {
            lock (this)
            {
                currentSearch = null;
                currentSearch = CreateSearch();
            }
        }

        public IEnumerable<NamespaceInfo> GetNamespaces()
        {
            var search = GetSearch();
            return search.Namespaces;
        }
    }
}
