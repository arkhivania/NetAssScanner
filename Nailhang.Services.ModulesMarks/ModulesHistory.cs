using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nailhang.Services.Interfaces;
using Nailhang.Services.Interfaces.History;
using Orleans;
using Orleans.CodeGeneration;
using Orleans.Providers;
using Orleans.Serialization;

namespace Nailhang.Services.ModulesMarks
{
    class ModuleState
    {
        public Dictionary<int, Change> Changes { get; } = new Dictionary<int, Change>();
        public int SVersion { get; set; }
    }

    [StorageProvider(ProviderName = "GlobalDB")]
    class ModulesHistory : Orleans.Grain<ModuleState>, IModulesHistory
    {
        const int STOREVERSION = 2;

        private readonly IGrainFactory grainFactory;
        private readonly ILogger<ModulesHistory> logger;

        public ModulesHistory(IGrainFactory grainFactory, ILogger<ModulesHistory> logger)
        {
            this.grainFactory = grainFactory;
            this.logger = logger;
        }

        protected override async Task ReadStateAsync()
        {
            try
            {
                await base.ReadStateAsync();
                if (State.SVersion < STOREVERSION)
                    State = new ModuleState();
            }catch
            {
                logger.LogError("error reading state");
                State = new ModuleState();
            }            
        }

        static int strSumm(string num)
        {
            int res = 0;
            for (int i = 0; i < num.Length; i++)
                res += (int)num[i];
            return res;
        }

        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();

            var nameSpace = this.GetPrimaryKeyString();
            var index = strSumm(nameSpace) % 100;

            var catalogGrain = grainFactory.GetGrain<INamespacesCatalog>(index);
            await catalogGrain.RegisterNamespace(nameSpace);
        }

        public Task<Change[]> GetChanges()
        {
            return Task.FromResult(State.Changes.Values.ToArray());
        }

        string GetNamespace(string name)
        {
            var name_split = name.Split('.');
            if (name_split.Length == 1)
                return null;
            return string.Join(".", name_split.Take(name_split.Length - 1));
        }

        public async Task StoreChangeToNamespace(Change change)
        {
            if (State.Changes.TryGetValue(change.Revision.Id, out Change v))
            {
                if ((v.Modification & change.Modification) != 0)
                    return;

                v.Modification = v.Modification | change.Modification;
                State.Changes[change.Revision.Id] = v;
            }
            else
                State.Changes.Add(change.Revision.Id, change);

            var parent = GetNamespace(this.GetPrimaryKeyString());
            if (parent != null)
                await grainFactory.GetGrain<IModulesHistory>(parent).StoreChangeToNamespace(change);

            State.SVersion = STOREVERSION;
            await WriteStateAsync();
            logger.LogInformation($"{this.GetPrimaryKeyString()} stored");
        }

        public async Task Delete()
        {
            State.Changes.Clear();
            await WriteStateAsync();
        }
    }
}
