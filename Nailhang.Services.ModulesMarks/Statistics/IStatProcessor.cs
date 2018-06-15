using Orleans;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nailhang.Services.ModulesMarks.Statistics
{
    public interface IStatProcessor : IGrainWithIntegerKey, IRemindable
    {
        Task<IGrainReminder> StartReminder(TimeSpan? p = null);
        Task RemoveReminder();

        Task UpdateStat();
    }
}
