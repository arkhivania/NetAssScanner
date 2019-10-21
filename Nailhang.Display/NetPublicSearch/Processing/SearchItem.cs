using Nailhang.IndexBase.PublicApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nailhang.Display.NetPublicSearch.Processing
{
    class SearchItem : Base.ISearchItem
    {
        private readonly SplitStringStore.Item fullClassName;

        public SearchItem(AssemblyPublic assemblyPublic, SplitStringStore.Item fullClassName, bool isPublic)
        {
            this.Assembly = assemblyPublic;
            this.fullClassName = fullClassName;
            this.IsPublic = isPublic;
        }

        public string FullClassName => fullClassName.ToString();
        public string Namespace => fullClassName.Parent == null ? "" : fullClassName.Parent.ToString();

        public bool IsPublic { get; }

        public string ClassName => fullClassName.Part;
        public AssemblyPublic Assembly { get; }
    }
}
