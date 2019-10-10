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
        readonly Base.SearchItem[] searchItems;

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

        readonly ISearch classSearch;
        readonly ISearch methodSearch;

        public NetSearch(IWSBuilder wSBuilder, IPublicApiStorage publicApiStorage)
        {
            searchItems = publicApiStorage.LoadAssemblies().SelectMany(a => Prepare(a))
                .ToArray();

            {
                var search_strings = searchItems.Select(w => w.ClassName);
                var stat = wSBuilder.BuildStat(search_strings);
                var index = wSBuilder.Index(search_strings.Select(w => new Bulk { Sentence = w }), stat);
                this.classSearch = wSBuilder.CreateSearch(index);
            }

            {
                var search_strings = searchItems.Select(w => w.MethodName);
                var stat = wSBuilder.BuildStat(search_strings);
                var index = wSBuilder.Index(search_strings.Select(w => new Bulk { Sentence = w }), stat);
                this.methodSearch = wSBuilder.CreateSearch(index);
            }
        }

        public IEnumerable<SearchItem> Search(string query)
        {
            var name = query;
            return classSearch.Search(new Request { Message = query })
                .Concat(methodSearch.Search(new Request { Message = query }))
                .OrderByDescending(w => w.Relevance)
                .Where(w => w.Relevance > 0.75)
                .Where(w => w.Relevance > 0.90f
                        || searchItems[w.DocumentIndex].ClassName.ToLower().Contains(query)
                        || searchItems[w.DocumentIndex].MethodName.ToLower().Contains(query))
                .Select(q => searchItems[q.DocumentIndex])
                .Take(100)
                .ToArray();
        }
    }
}
