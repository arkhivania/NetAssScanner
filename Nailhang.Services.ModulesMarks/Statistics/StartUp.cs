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

        public StartUp(IGrainFactory grainFactory)
        {
            this.grainFactory = grainFactory;
        }

        async Task IStartUp.StartUp()
        {
            var sp = grainFactory.GetGrain<IStatProcessor>(0);
            await sp.StartReminder(TimeSpan.FromMinutes(1));
        }
    }
}
