using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.System.Threading;
using MALClient.Adapters;
using MALClient.Models.Enums;

namespace MALClient.UWP.Adapters
{
    public class ScheduledJobsManager : ISchdeuledJobsManger
    {
        private readonly Dictionary<ScheduledJob, ThreadPoolTimer> _jobTimers = new Dictionary<ScheduledJob, ThreadPoolTimer>();
        private readonly Dictionary<ScheduledJob, Action> _jobs = new Dictionary<ScheduledJob, Action>();
         
        public void StartJob(ScheduledJob job, int reccurence, Action jobDefinition)
        {
            if (_jobTimers.ContainsKey(job))
            {
                var timer = _jobTimers[job];
                if (timer.Period.Minutes == reccurence)
                    return;

                timer.Cancel();
                _jobTimers.Remove(job);

            }

            _jobs[job] = jobDefinition;
            _jobTimers.Add(job, ThreadPoolTimer.CreatePeriodicTimer(timer => _jobs[job].Invoke(),TimeSpan.FromMinutes(reccurence)));
        }

        public void StopJob(ScheduledJob job)
        {
            if (_jobTimers.ContainsKey(job))
            {
                _jobTimers[job].Cancel();
                _jobTimers.Remove(job);
            }
        }
    }
}
