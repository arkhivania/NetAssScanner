using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nailhang.Services.Interfaces
{
    public interface INamespaces : IGrainWithIntegerKey
    {
        Task<History.IModulesHistory> GetNamespace(string @namespace);
    }
}
