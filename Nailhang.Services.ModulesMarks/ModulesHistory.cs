using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nailhang.IndexBase.History.Base;
using Nailhang.Services.Interfaces;
using Orleans;
using Orleans.Providers;

namespace Nailhang.Services.ModulesMarks
{
    class ModuleState
    {
        public Dictionary<int, Change> Changes { get; } = new Dictionary<int, Change>();
        public int SVersion { get; set; }
    }

    [StorageProvider(ProviderName = "ModulesStoreProvider")]
    class ModulesHistory : Orleans.Grain<ModuleState>, IModulesHistory
    {
        const int STOREVERSION = 2;

        private readonly IGrainFactory grainFactory;

        public ModulesHistory(IGrainFactory grainFactory)
        {
            this.grainFactory = grainFactory;
        }
        
        public override Task OnActivateAsync()
        {
            if (State.SVersion < STOREVERSION)
                State = new ModuleState();

            return base.OnActivateAsync();
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

            var parent = GetNamespace(this.GetGrainIdentity().PrimaryKeyString);
            if (parent != null)
                await grainFactory.GetGrain<IModulesHistory>(parent).StoreChangeToNamespace(change);

            State.SVersion = STOREVERSION;
            await WriteStateAsync();
        }
    }
}
