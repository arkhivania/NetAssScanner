using Nailhang.Services.ModulesMarks.HotModulesBuilder.Base;
using Orleans;
using Orleans.Providers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nailhang.Services.ModulesMarks.OrleansHotModules
{
    [StorageProvider(ProviderName = "GlobalDB")]
    class HotModules : Grain<HotStore>, IHotModules
    {
        public Task<HotInfo[]> GetInfos()
        {
            return Task.FromResult(State.Infos);
        }

        public async Task Update(HotInfo[] hotInfos)
        {
            State = new HotStore { Infos = hotInfos };
            await base.WriteStateAsync();
        }
    }
}
