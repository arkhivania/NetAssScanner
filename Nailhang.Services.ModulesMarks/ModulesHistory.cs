﻿using System;
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
        bool changed = false;

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
            }
            catch
            {
                logger.LogError("error reading state");
                State = new ModuleState();
            }
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

        public override async Task OnDeactivateAsync()
        {
            if (changed)
            {
                await WriteStateAsync();
                logger.LogInformation($"{this.GetPrimaryKeyString()} stored on deactivation");
                changed = false;
                timerWriteID = Guid.Empty;
            }

            await base.OnDeactivateAsync();
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
            
            SaveTimerRun();

            var parent = GetNamespace(this.GetPrimaryKeyString());
            if (parent != null)
                await grainFactory.GetGrain<IModulesHistory>(parent).StoreChangeToNamespace(change);

            State.SVersion = STOREVERSION;
        }

        Guid timerWriteID = Guid.Empty;
        private void SaveTimerRun()
        {
            changed = true;
            timerWriteID = Guid.NewGuid();

            var capID = timerWriteID;

            Task.Factory.StartNew(async () =>
            {
                if (capID != timerWriteID)
                    return;

                await Task.Delay(TimeSpan.FromSeconds(15));
                if (capID != timerWriteID)
                    return;

                await this.AsReference<IModulesHistory>().WriteChanges();
            });            
        }

        public Task Delete()
        {
            State.Changes.Clear();
            SaveTimerRun();

            return Task.CompletedTask;
        }

        public async Task WriteChanges()
        {
            await WriteStateAsync();
            changed = false;
            logger.LogInformation($"{this.GetPrimaryKeyString()} stored on write request");
        }
    }
}
