using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.Services.Interfaces.History
{
    public struct Revision
    {
        public int Id { get; set; }
        public string User { get; set; }
        public DateTime UtcDateTime { get; set; }
    }
}
