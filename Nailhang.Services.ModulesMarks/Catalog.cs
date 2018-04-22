using Nailhang.Services.Interfaces;
using Orleans;
using Orleans.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailhang.Services.ModulesMarks
{
    class CatalogState
    {
        public HashSet<string> Namespaces { get; } = new HashSet<string>();
    }

    [StorageProvider(ProviderName = "GlobalDB")]
    class Catalog : Grain<CatalogState>, INamespacesCatalog
    {
        public Task<string[]> GetNamespaces()
        {
            return Task.FromResult(State.Namespaces.ToArray());
        }

        public override Task OnActivateAsync()
        {
            return base.OnActivateAsync();
        }

        public async Task RegisterNamespace(string @namespace)
        {
            if (State.Namespaces.Add(@namespace))
                await this.WriteStateAsync();
        }

        public async Task RemoveNamespace(string @namespace)
        {
            if(State.Namespaces.Remove(@namespace))
                await this.WriteStateAsync();
        }
    }
}
