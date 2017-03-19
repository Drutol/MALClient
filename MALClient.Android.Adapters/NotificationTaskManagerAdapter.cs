using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MALClient.Adapters;
using MALClient.Models.Enums;

namespace MALClient.Android.Adapters
{
    public class NotificationTaskManagerAdapter : INotificationsTaskManager
    {
        public void StartTask(BgTasks task)
        {
            
        }

        public void StopTask(BgTasks task)
        {
           // throw new NotImplementedException();
        }

        public void CallTask(BgTasks task)
        {
            //throw new NotImplementedException();
        }
    }
}