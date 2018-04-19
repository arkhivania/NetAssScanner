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
        [LiteDB.BsonId]
        public string Namespace { get; set; }

        public Dictionary<int, Change> Changes { get; set; } = new Dictionary<int, Change>();
        public int SVersion { get; set; }
    }

    class ModulesHistory : Orleans.Grain, IModulesHistory
    {
        const int STOREVERSION = 2;

        private readonly IGrainFactory grainFactory;
        private readonly LiteDB.LiteDatabase liteDatabase;
        private readonly ILogger<ModulesHistory> logger;
        ModuleState state = new ModuleState();

        public ModulesHistory(IGrainFactory grainFactory, 
            LiteDB.LiteDatabase liteDatabase, ILogger<ModulesHistory> logger)
        {
            this.grainFactory = grainFactory;
            this.liteDatabase = liteDatabase;
            this.logger = logger;
        }

        public override async Task OnActivateAsync()
        {
            var key = this.GetPrimaryKeyString();
            var c = liteDatabase.GetCollection<ModuleState>();
            var ms = c.FindOne(q => q.Namespace == key);

            if (ms == null || ms.SVersion < STOREVERSION)
                state = new ModuleState { Namespace = key };
            else
                state = ms;
            
            await base.OnActivateAsync();
        }

        public Task<Change[]> GetChanges()
        {
            return Task.FromResult(state.Changes.Values.ToArray());
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
            if (state.Changes.TryGetValue(change.Revision.Id, out Change v))
            {
                if ((v.Modification & change.Modification) != 0)
                    return;

                v.Modification = v.Modification | change.Modification;
                state.Changes[change.Revision.Id] = v;
            }
            else
                state.Changes.Add(change.Revision.Id, change);

            var parent = GetNamespace(this.GetGrainIdentity().PrimaryKeyString);
            if (parent != null)
                await grainFactory.GetGrain<IModulesHistory>(parent).StoreChangeToNamespace(change);

            state.SVersion = STOREVERSION;
            var c = liteDatabase.GetCollection<ModuleState>();
            var ms = c.Upsert(state);
            logger.LogInformation($"{state.Namespace} stored");
        }
    }
}
