using Microsoft.Extensions.Configuration;
using Nailhang.Services.Interfaces;
using Nailhang.Services.ModulesMarks.HotModulesBuilder.Base;
using Orleans;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailhang.Services.ModulesMarks.Statistics
{
    class Processor : Grain, IStatProcessor
    {
        private readonly IGrainFactory grainFactory;
        private readonly IConfigurationRoot configurationRoot;

        public Processor(IGrainFactory grainFactory, IConfigurationRoot configurationRoot)
        {
            this.grainFactory = grainFactory;
            this.configurationRoot = configurationRoot;
        }

        public async Task RemoveReminder()
        {
            var reminderType = await GetReminder("main");
            await UnregisterReminder(reminderType);
        }

        public async Task<IGrainReminder> StartReminder(TimeSpan? p = null)
        {
            var usePeriod = p ?? TimeSpan.FromMinutes(10);
            return await RegisterOrUpdateReminder("main", usePeriod, usePeriod);
        }

        public async Task BuildStat()
        {
            var daysCount = configurationRoot.GetSection("StatHotDaysCount")?.Get<int>() ?? 7;

            for (int i = 0; i < 100; ++i)
            {   
                var namespaces = grainFactory.GetGrain<INamespaces>(0);
                var catalog = await grainFactory.GetGrain<INamespacesCatalog>(i).GetNamespaces();
                var hotInfos = new List<HotInfo>();
                foreach (var n in catalog)
                {
                    var @namespace = await namespaces.GetNamespace(n);
                    var changes = await @namespace.GetChanges();

                    var now = DateTime.UtcNow;
                    var newChanges = changes
                        .Where(q => (q.Modification & Interfaces.History.Modification.Modification) != 0)
                        .Where(q => (now - q.Revision.UtcDateTime) < TimeSpan.FromDays(daysCount)).ToArray();
                    if (newChanges.Length > 0)
                        hotInfos.Add(new HotInfo { Module = n, LastRevisions = newChanges });
                }

                await grainFactory.GetGrain<OrleansHotModules.IHotModules>(i).Update(hotInfos.ToArray());
            }
        }

        public Task ReceiveReminder(string reminderName, TickStatus status)
        {
            return BuildStat();
        }

        public Task UpdateStat()
        {
            return BuildStat();
        }
    }
}
