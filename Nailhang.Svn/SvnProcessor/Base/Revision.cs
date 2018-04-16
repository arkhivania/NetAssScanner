using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.Svn.SvnProcessor.Base
{
    public struct Revision
    {
        public int Number { get; set; }
        public string User { get; set; }
        public DateTime UtcDateTime { get; set; }

        public override string ToString()
        {
            return $"{UtcDateTime.ToLocalTime()} {Number} {User}";
        }
    }
}
