using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using Java.Lang;
using Debug = System.Diagnostics.Debug;

namespace MALClient.Android
{
    public class MemoryWatcher
    {
        private Timer _memoryCheckTimer;

        #region Singleton

        private MemoryWatcher()
        {
        }

        private static MemoryWatcher _watcher;
        public static MemoryWatcher Watcher => _watcher ?? (_watcher = new MemoryWatcher());

        #endregion

        private void CreateTimer(TimeSpan dueTime)
        {
            _memoryCheckTimer = new Timer(OnMemoryCheck, null, dueTime, TimeSpan.FromSeconds(15));
        }

        private void OnMemoryCheck(object state)
        {

            var available = Runtime.GetRuntime().MaxMemory();
            var used = Runtime.GetRuntime().TotalMemory();

            float percentAvailable = 100f * (1f - ((float)used / available));
            Debug.WriteLine($">>>> MEMORY {used}/{available} ({percentAvailable}%) <<<<");
            if (percentAvailable <= 7.5)
            {
                ImageService.Instance.InvalidateMemoryCache();
                AnimeImageExtensions.NotifyCacheWiped();
                Runtime.GetRuntime().Gc();
                Pause();
                Resume(true);
            }
        }

        public void Pause()
        {
            _memoryCheckTimer?.Dispose();
            _memoryCheckTimer = null;
        }

        public void Resume(bool withDueTime)
        {
            if(_memoryCheckTimer == null)
                CreateTimer(withDueTime ? TimeSpan.FromSeconds(30) :  TimeSpan.Zero);
        }

    }
}