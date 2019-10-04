using Ninject;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.Display.InterfacesSearch
{
    public class Module : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<Processing.StorageSearch>()
                .ToSelf();

            DateTime? lastCreatedTime = null;
            Base.IInterfacesSearch created = null;

            Kernel.Bind<Base.IInterfacesSearch>()
                .ToMethod(q => 
                {
                    var curTime = DateTime.UtcNow;
                    if(created == null || (curTime - lastCreatedTime.Value) > TimeSpan.FromSeconds(15))
                    {
                        created = q.Kernel.Get<Processing.StorageSearch>();
                        lastCreatedTime = curTime;
                    }

                    return created;
                });
        }
    }
}
