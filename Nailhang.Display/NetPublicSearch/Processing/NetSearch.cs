using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nailhang.Display.NetPublicSearch.Base;
using Nailhang.Display.Tools.TextSearch.Base;
using Nailhang.IndexBase.PublicApi;

namespace Nailhang.Display.NetPublicSearch.Processing
{
    class NetSearch : Base.INetSearch
    {
        struct Searcher 
        {
            public Base.SearchItem[] SearchItems { get; set; }
            public ISearch ClassSearch { get; set; }
            public ISearch MethodSearch { get; set; }
        }

        readonly Task<Searcher> searcher;

        

        static IEnumerable<Base.SearchItem> Prepare(AssemblyPublic assemblyPublic)
        {
            foreach (var c in assemblyPublic.Classes)
                foreach (var m in c.Methods)
                {
                    yield return new SearchItem
                    {
                        AssemblyName = assemblyPublic.FullName,
                        AssemblyVersion = assemblyPublic.AssemblyVersion.ToString(),
                        GenericString = m.GenericParameters.Length > 0 ? string.Join(", ", m.GenericParameters) : "",
                        MethodName = m.Name,
                        ParametersString = m.Parameters.Length > 0 ? string.Join(", ", m.Parameters.Select(p => $"{p.Type} {p.Name}")) : "",
                        ResultType = m.Returns,
                        ClassName = c.Name
                    };
                }
        }

        Searcher Create(IWSBuilder wSBuilder, IPublicApiStorage publicApiStorage)
        {
            var res = new Searcher();
            res.SearchItems = publicApiStorage.LoadAssemblies().SelectMany(a => Prepare(a))
                .ToArray();

            {
                var search_strings = res.SearchItems.Select(w => w.ClassName);
                var stat = wSBuilder.BuildStat(search_strings);
                var index = wSBuilder.Index(search_strings.Select(w => new Bulk { Sentence = w }), stat);
                res.ClassSearch = wSBuilder.CreateSearch(index);
            }

            {
                var search_strings = res.SearchItems.Select(w => w.MethodName);
                var stat = wSBuilder.BuildStat(search_strings);
                var index = wSBuilder.Index(search_strings.Select(w => new Bulk { Sentence = w }), stat);
                res.MethodSearch = wSBuilder.CreateSearch(index);
            }

            return res;
        }

        public NetSearch(IWSBuilder wSBuilder, IPublicApiStorage publicApiStorage)
        {
            this.searcher = Task.Factory
                .StartNew(() => Create(wSBuilder, publicApiStorage), TaskCreationOptions.LongRunning);
        }

        public IEnumerable<SearchItem> Search(string query)
        {
            var sr = searcher.Result;
            
            var name = query;
            return sr.ClassSearch.Search(new Request { Message = query })
                .Concat(sr.MethodSearch.Search(new Request { Message = query }))
                .OrderByDescending(w => w.Relevance)
                .Where(w => w.Relevance > 0.50)
                .Where(w => w.Relevance > 0.75f
                        || sr.SearchItems[w.DocumentIndex].ClassName.ToLower().Contains(query)
                        || sr.SearchItems[w.DocumentIndex].MethodName.ToLower().Contains(query))
                .Select(q => sr.SearchItems[q.DocumentIndex])
                .Take(200)
                .ToArray();
        }
    }
}
