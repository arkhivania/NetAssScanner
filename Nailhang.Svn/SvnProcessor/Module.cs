using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.Svn.SvnProcessor
{
    public class Module : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<Base.ISvn>().To<Processing.Svn>();
            Kernel.Bind<Base.Settings>().ToConstant(new Base.Settings
            {
            });
        }
    }
}
