using Nailhang.IndexBase.History.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nailhang.Services.Interfaces
{
    public interface IModulesHistory : Orleans.IGrainWithStringKey
    {
        Task<Change[]> GetChanges();
        Task StoreChangeToNamespace(Change change);
    }
}
