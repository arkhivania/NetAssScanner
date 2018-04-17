using Microsoft.Extensions.Configuration;
using Ninject;
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
            Kernel.Bind<Base.Settings>()
                .ToMethod(q => 
                {
                    var config = q.Kernel.Get<IConfiguration>();

                    int? codePage = null;
                    var pageSettings = config["codePage"];
                    if (!string.IsNullOrEmpty(pageSettings))
                        codePage = int.Parse(pageSettings);

                    return new Base.Settings { CodePage = codePage };
                });
        }
    }
}
