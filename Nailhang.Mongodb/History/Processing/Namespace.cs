using Nailhang.IndexBase.History.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.Mongodb.History.Processing
{
    public class Namespace
    {
        public string Id { get; set; }
        public List<Revision> Revisions { get; set; } = new List<Revision>();
    }
}
