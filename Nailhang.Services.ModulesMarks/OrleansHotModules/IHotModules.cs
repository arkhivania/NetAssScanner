using Nailhang.Services.ModulesMarks.HotModulesBuilder.Base;
using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nailhang.Services.ModulesMarks.OrleansHotModules
{
    public interface IHotModules : IGrainWithIntegerKey
    {
        Task<HotInfo[]> GetInfos();
        Task Update(HotInfo[] hotInfos);
    }
}
