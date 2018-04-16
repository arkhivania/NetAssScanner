using System;
using System.Collections.Generic;
using System.Text;
using Nailhang.Svn.SvnProcessor.Base;

namespace Nailhang.Svn.SvnProcessor.Processing
{
    class Svn : Base.ISvn
    {
        private readonly Settings settings;

        public Svn(Base.Settings settings)
        {
            this.settings = settings;
        }

        public ISvnConnection Connect(string url)
        {
            return new SvnConnection(url, settings);
        }
    }
}
