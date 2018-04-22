using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nailhang.Services.Interfaces
{
    public interface INamespacesCatalog : IGrainWithIntegerKey
    {
        Task RegisterNamespace(string @namespace);
        Task RemoveNamespace(string @namespace);
        Task<string[]> GetNamespaces();
    }
}
