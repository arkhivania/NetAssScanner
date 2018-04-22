using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nailhang.Services.Interfaces.History
{
    public interface IModulesHistory : Orleans.IGrainWithStringKey
    {
        Task<Change[]> GetChanges();
        Task StoreChangeToNamespace(Change change);
        Task Delete();
    }
}
