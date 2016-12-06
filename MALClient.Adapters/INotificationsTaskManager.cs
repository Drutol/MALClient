using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MALClient.Models.Enums;

namespace MALClient.Adapters
{
    public interface INotificationsTaskManager
    {
        void StartTask(BgTasks task);
        void StopTask(BgTasks task);
        void CallTask(BgTasks task);
    }
}
