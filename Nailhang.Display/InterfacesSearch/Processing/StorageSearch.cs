using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nailhang.Display.Controllers;
using Nailhang.Display.Models;
using Nailhang.Display.Tools.TextSearch.Base;

namespace Nailhang.Display.InterfacesSearch.Processing
{
    class StorageSearch : Base.IInterfacesSearch
    {
        readonly ISearch search;
        readonly InterfaceMD5KV[] interfaces;

        public StorageSearch(IWSBuilder wSBuilder, InterfaceController interfaceController)
        {
            var model = interfaceController.GetInterfacesModel("");
            this.interfaces = model.Interfaces.ToArray();
            var stat = wSBuilder.BuildStat(interfaces.Select(w => w.Name));
            var index = wSBuilder.Index(interfaces.Select(w => new Bulk { Sentence = w.Name }), stat);
            this.search = wSBuilder.CreateSearch(index);
        }

        public IEnumerable<InterfaceMD5KV> Search(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new InvalidOperationException("Name can't be empty");

            var resp = search
                .Search(new Request { Message = name })
                .OrderByDescending(w => w.Relevance)
                .Where(w => w.Relevance > 0.1f)
                .ToArray();

            var res = resp
                .Select(w => interfaces[w.DocumentIndex])
                .ToArray();

            return res;
        }
    }
}
