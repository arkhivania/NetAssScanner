using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.Svn.SvnProcessor.Base
{
    public struct Change
    {
        public string Path { get; set; }
        public ChangeType ChangeType { get; set; }
        public int Revision { get; set; }
    }
}
