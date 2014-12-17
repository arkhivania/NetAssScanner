﻿using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailhang.Processing
{
    [Nailhang.Module]
    [Nailhang.ModuleDescription("Модуль сканирования сборок с помощью библиотеки Mono Cecil")]
    public class CecilModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IndexBase.Index.IIndexProcessor>().To<CecilProcessor>();

            Kernel.Bind<ModuleBuilder.Base.IModuleBuilder>().To<ModuleBuilder.NailhangModuleBuilder>();
            Kernel.Bind<ModuleBuilder.Base.IModuleBuilder>().To<ModuleBuilder.NamespaceGroupedBuilder>();
        }
    }
}