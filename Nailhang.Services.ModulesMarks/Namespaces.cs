using Nailhang.Services.Interfaces;
using Nailhang.Services.Interfaces.History;
using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nailhang.Services.ModulesMarks
{
    class Namespaces : Grain, INamespaces
    {
        private readonly IGrainFactory grainFactory;

        public Namespaces(IGrainFactory grainFactory)
        {
            this.grainFactory = grainFactory;
        }

        static int strSumm(string num)
        {
            int res = 0;
            for (int i = 0; i < num.Length; i++)
                res += (int)num[i];
            return res;
        }

        INamespacesCatalog GetCatalog(string @namespace)
        {
            var index = strSumm(@namespace) % 100;
            var catalogGrain = grainFactory.GetGrain<INamespacesCatalog>(index);
            return catalogGrain;
        }

        public async Task<IModulesHistory> GetNamespace(string @namespace)
        {
            await GetCatalog(@namespace).RegisterNamespace(@namespace);
            return grainFactory.GetGrain<IModulesHistory>(@namespace);
        }
    }
}
