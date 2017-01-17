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
    public class ScheduledJobsManager : ISchdeuledJobsManger
    {
        public void StartJob(ScheduledJob job, int reccurence, Action jobDefinition)
        {
            //throw new NotImplementedException();
        }

        public void StopJob(ScheduledJob job)
        {
           // throw new NotImplementedException();
        }
    }
}