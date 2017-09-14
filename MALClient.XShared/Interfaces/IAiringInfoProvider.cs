using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.XShared.Interfaces
{
    public interface IAiringInfoProvider
    {
        Task Init(bool cacheOnly);
        bool TryGetCurrentEpisode(int id, out int episode, DateTime? forDay = null);
        bool TryGetNextAirDate(int id, DateTime forDay, out DateTime date);
        bool TryGetAiringDay(int id,out DayOfWeek day);

        bool InitializationSuccess { get; }
    }
}
