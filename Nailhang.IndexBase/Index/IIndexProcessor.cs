using Nailhang.IndexBase.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailhang.IndexBase.Index
{
    public struct ExtractResult
    {
        public Zone? Zone { get; set; }
        public Module Module { get; set; }
    }

    public interface IIndexProcessor
    {
        IEnumerable<ExtractResult> ExtractModules(string filePath);
    }
}
