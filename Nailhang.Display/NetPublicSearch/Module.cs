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
            var created = new List<Base.INetSearch>();

            Kernel.Bind<Base.INetSearch>()
                .ToMethod(q =>
                {
                    var curTime = DateTime.UtcNow;
                    Task createTask = null;
                    lock (created)
                        if (created.Count == 0
                        || (curTime - lastCreatedTime.Value) > TimeSpan.FromMinutes(1))
                        {
                            createTask = Task.Factory.StartNew(() =>
                            {
                                var cr = q.Kernel.Get<Processing.NetSearch>();
                                lock (created)
                                {
                                    created.Insert(0, cr);
                                    while (created.Count > 1)
                                        created.RemoveAt(0);
                                }
                            });

                            lastCreatedTime = curTime;
                        }

                    lock (created)
                        if (created.Any())
                            return created.Last();

                    createTask.ConfigureAwait(false)
                        .GetAwaiter().GetResult();
                    return created.First();
                });
        }
    }
}
