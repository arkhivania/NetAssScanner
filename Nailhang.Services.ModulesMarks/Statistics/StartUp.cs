using Microsoft.Extensions.Configuration;
using Nailhang.Services.Interfaces;
using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nailhang.Services.ModulesMarks.Statistics
{
    class StartUp : IStartUp
    {
        private readonly IGrainFactory grainFactory;
        private readonly IConfigurationRoot configurationRoot;

        public StartUp(IGrainFactory grainFactory, IConfigurationRoot configurationRoot)
        {
            this.grainFactory = grainFactory;
            this.configurationRoot = configurationRoot;
        }

        async Task IStartUp.StartUp()
        {
            int stat_update_minutes = configurationRoot.GetSection("StatRebuildPeriodMinutes")?.Get<int>() ?? 15;

            var sp = grainFactory.GetGrain<IStatProcessor>(0);
            await sp.StartReminder(TimeSpan.FromMinutes(stat_update_minutes));
        }
    }
}
