using Nailhang.Services.Interfaces;
using Orleans;
using Orleans.Providers;
using System;
using System.Collections.Generic;
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
        public async Task RegisterNamespace(string @namespace)
        {
            if (State.Namespaces.Add(@namespace))
                await this.WriteStateAsync();
        }
    }
}
