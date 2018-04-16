using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.Svn.SvnProcessor.Base
{
    public interface ISvn
    {
        ISvnConnection Connect(string url);
    }
}
