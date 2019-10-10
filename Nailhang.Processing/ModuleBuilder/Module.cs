using Nailhang.Processing.ModuleBuilder.Processing;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailhang.Processing.ModuleBuilder
{
    [Module]
    [ModuleDescription("Модуль сканирования сборок с помощью библиотеки Mono Cecil")]
    public class Module : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IndexBase.Index.IIndexProcessor>().To<CecilProcessor>();
            Kernel.Bind<Base.IModuleBuilder>().To<NailhangModuleBuilder>();
        }
    }
}
