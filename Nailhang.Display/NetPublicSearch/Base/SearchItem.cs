using Nailhang.IndexBase.PublicApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nailhang.Display.NetPublicSearch.Base
{
    public struct SearchItem
    {
        public SearchItem(AssemblyPublic assemblyPublic, string fullClassName, bool isPublic)
        {
            this.Assembly = assemblyPublic;
            splited = fullClassName.Split('.');
            this.IsPublic = isPublic;
        }

        readonly string[] splited;

        public string FullClassName => string.Join(".", splited);
        public string Namespace => splited.Length > 1 ? string.Join(".", splited.Take(splited.Length - 1)) : "";

        public bool IsPublic { get; }

        public string ClassName => splited[splited.Length - 1];
        public AssemblyPublic Assembly { get; }
    }
}
