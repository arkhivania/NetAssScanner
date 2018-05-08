using Nailhang.Services.Interfaces;
using Nailhang.Services.Interfaces.Statistics;
using Orleans;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nailhang.Services.ModulesMarks.Statistics
{
    class Processor : Grain, IStatProcessor
    {
        private readonly IGrainFactory grainFactory;

        public Processor(IGrainFactory grainFactory)
        {
            this.grainFactory = grainFactory;
        }

        public async Task RemoveReminder()
        {
            var reminderType = await GetReminder("main");
            await UnregisterReminder(reminderType);
        }

        public async Task<IGrainReminder> StartReminder(TimeSpan? p = null)
        {
            var usePeriod = p ?? TimeSpan.FromMinutes(10);
            IGrainReminder r = null;
            return await RegisterOrUpdateReminder("main", usePeriod - TimeSpan.FromSeconds(2), usePeriod);
        }

        public async Task BuildStat()
        {
            for (int i = 0; i < 100; ++i)
            {
                var namespaces = grainFactory.GetGrain<INamespaces>(0);
                var catalog = await grainFactory.GetGrain<INamespacesCatalog>(i).GetNamespaces();
                //foreach (var n in catalog)
                //{
                //    var @namespace = await namespaces.GetNamespace(n);
                //    var changes = await @namespace.GetChanges();
                //}
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
