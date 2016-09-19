using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Adapters
{
    public interface INotificationsTaskManager
    {
        void StartTask();
        void StopTask();
    }
}
