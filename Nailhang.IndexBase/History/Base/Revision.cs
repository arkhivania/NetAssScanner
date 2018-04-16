using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.IndexBase.History.Base
{
    public struct Revision
    {
        public int Id { get; set; }
        public string User { get; set; }
        public DateTime DateTime { get; set; }
    }
}
