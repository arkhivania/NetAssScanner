using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailhang.Mongodb
{
    [Nailhang.Module]
    [Nailhang.ModuleDescription("Модуль хранения данных Nailhang в БД Mongo")]
    public class Module : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<Nailhang.IndexBase.Storage.IModulesStorage>().To<MongoStorage>();
        }
    }
}
