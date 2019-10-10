using Ninject;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nailhang.Display.NetPublicSearch
{
    public class Module : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<Processing.NetSearch>()
                .ToSelf();

            DateTime? lastCreatedTime = null;
            Base.INetSearch created = null;

            Kernel.Bind<Base.INetSearch>()
                .ToMethod(q =>
                {
                    var curTime = DateTime.UtcNow;
                    if (created == null || (curTime - lastCreatedTime.Value) > TimeSpan.FromSeconds(15))
                    {
                        created = q.Kernel.Get<Processing.NetSearch>();
                        lastCreatedTime = curTime;
                    }

                    return created;
                });
        }
    }
}
