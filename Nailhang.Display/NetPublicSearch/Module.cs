using Ninject;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    if (created == null || (curTime - lastCreatedTime.Value) > TimeSpan.FromMinutes(1))
                    {
                        created = q.Kernel.Get<Processing.NetSearch>();
                        lastCreatedTime = curTime;
                    }

                    return created;
                });
        }
    }
}
