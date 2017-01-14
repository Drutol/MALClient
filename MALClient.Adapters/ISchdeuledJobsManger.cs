using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MALClient.Models.Enums;

namespace MALClient.Adapters
{
    public interface ISchdeuledJobsManger
    {
        void StartJob(ScheduledJob job, int reccurence,Action jobDefinition);
        void StopJob(ScheduledJob job);
    }
}
