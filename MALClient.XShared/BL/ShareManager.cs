using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using MALClient.Models.Enums;
using MALClient.Models.Models.Anime;
using MALClient.XShared.Interfaces;
using MALClient.XShared.Utils;

namespace MALClient.XShared.BL
{
    public class ShareManager : IShareManager
    {
        private Timer _countdownTimer;

        private List<(ShareEvent e, AnimeShareDiff diff)> _lastEvents
            = new List<(ShareEvent e, AnimeShareDiff diff)>();

        private bool _shareTimerRunning;

        public event EventHandler<bool> TimerStateChanged; 

        public bool ShareTimerRunning
        {
            get => _shareTimerRunning;
            set
            {
                _shareTimerRunning = value;
                TimerStateChanged?.Invoke(this,value);
            }
        }

        public void EnqueueEvent(ShareEvent action, AnimeShareDiff diff)
        {
            if(!Settings.EnableShareButton)
                return;

            RestartTimer();

            if (_lastEvents.Any(tuple => tuple.diff.Id != diff.Id))
                _lastEvents.Clear();

            var entry = _lastEvents.FirstOrDefault(tuple => tuple.e == action);

            if (entry != default)
                _lastEvents.Remove(entry);

            switch (action)
            {
                case ShareEvent.AnimeScoreChanged when diff.NewScore == 0:
                case ShareEvent.AnimeEpisodesChanged when diff.NewEpisodes == 0:
                    return;
            }

            _lastEvents.Add((action, diff));
        }

        string IShareManager.GenerateMessage()
        {
            return $"{GenerateMessage()}\n" +
                   $"#MALClient";
        }

        private string GenerateMessage()
        {
            var events = _lastEvents.Aggregate(ShareEvent.None, (t, x) => t | x.e);

            var epDiff = _lastEvents.FirstOrDefault(tuple => tuple.e == ShareEvent.AnimeEpisodesChanged);
            var scoreDiff = _lastEvents.FirstOrDefault(tuple => tuple.e == ShareEvent.AnimeScoreChanged);
            var statusDiff = _lastEvents.FirstOrDefault(tuple => tuple.e == ShareEvent.AnimeStatusChanged);

            _lastEvents.Clear();
            StopTimer();

            if (events.HasFlag(ShareEvent.AnimeScoreChanged) &&
                events.HasFlag(ShareEvent.AnimeStatusChanged))
            {
                return
                    $"I've {FormatStatus(statusDiff.diff.NewStatus, statusDiff.diff.IsAnime)} {epDiff.diff.Title} with score of {scoreDiff.diff.NewScore}/10.\n" +
                    $"{epDiff.diff.Url}";
            }

            if (events.HasFlag(ShareEvent.AnimeScoreChanged) &&
                events.HasFlag(ShareEvent.AnimeEpisodesChanged))

            {
                return
                    $"I've {WatchedOrRead()} {epDiff.diff.NewEpisodes}/{FormatAllEpisodes(epDiff.diff.TotalEpisodes)} {EpisodesOrChapters()} of {epDiff.diff.Title} with score of {scoreDiff.diff.NewScore}/10.\n" +
                    $"{epDiff.diff.Url}";
            }

            if (events.HasFlag(ShareEvent.AnimeEpisodesChanged) &&
                events.HasFlag(ShareEvent.AnimeStatusChanged))

            {
                return
                    $"I've {FormatStatus(statusDiff.diff.NewStatus, statusDiff.diff.IsAnime)} {epDiff.diff.Title} with {epDiff.diff.NewEpisodes}/{epDiff.diff.TotalEpisodes} {EpisodesOrChapters()} {WatchedOrRead()}.\n" +
                    $"{epDiff.diff.Url}";
            }

            if (events.HasFlag(ShareEvent.AnimeEpisodesChanged))
            {
                return
                    $"I've just {WatchedOrRead()} the {FormatEpisode(epDiff.diff.NewEpisodes)} {EpisodeOrChapter()} of {epDiff.diff.Title}.\n" +
                    $"{epDiff.diff.Url}";

                //$"I've just {WatchedOrRead()} {epDiff.diff.NewEpisodes}/{FormatAllEpisodes(epDiff.diff.TotalEpisodes)} {EpisodesOrChapters()} of {epDiff.diff.Title}.\n" +
            }

            if (events.HasFlag(ShareEvent.AnimeStatusChanged))
            {
                return
                    $"I've {FormatStatus(statusDiff.diff.NewStatus, statusDiff.diff.IsAnime)} {epDiff.diff.Title}\n" +
                    $"{epDiff.diff.Url}";
            }

            if (events.HasFlag(ShareEvent.AnimeScoreChanged))
            {
                return
                    $"I've scored {scoreDiff.diff.Title} {scoreDiff.diff.NewScore}/10.\n" +
                    $"{scoreDiff.diff.Url}";
            }

            return null;

            string WatchedOrRead() => epDiff.diff.IsAnime ? "watched" : "read";
            string EpisodesOrChapters() => epDiff.diff.IsAnime ? "episodes" : "chapters";
            string EpisodeOrChapter() => epDiff.diff.IsAnime ? "episode" : "chapter";
        }

        private void CountdownTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            _lastEvents.Clear();
            StopTimer();
        }

        private string FormatAllEpisodes(int eps) => eps <= 0 ? "?" : eps.ToString();

        private string FormatEpisode(int ep)
        {
            switch (ep)
            {
                case 1:
                    return "1st";
                case 2:
                    return "2nd";
                case 3:
                    return "3rd";
                default:
                    return $"{ep}th";
            }
        }

        private string FormatStatus(AnimeStatus status, bool isAnime)
        {
            switch (status)
            {
                case AnimeStatus.Watching:
                    return isAnime ? "started watching" : "started reading";
                case AnimeStatus.Completed:
                    return "completed";
                case AnimeStatus.OnHold:
                    return "put on hold";
                case AnimeStatus.Dropped:
                    return "dropped";
                case AnimeStatus.PlanToWatch:
                    return isAnime ? "added to PTW" : "added to PTR";
            }

            return null;
        }


        private void RestartTimer()
        {
            if (_countdownTimer != null)
            {
                _countdownTimer.Stop();
                _countdownTimer.Close();
                _countdownTimer = null;
            }

            _countdownTimer = new Timer(10000);
            _countdownTimer.Elapsed += CountdownTimerOnElapsed;
            _countdownTimer.Start();
            if(!ShareTimerRunning)
                ShareTimerRunning = true;
        }

        private void StopTimer()
        {
            if (_countdownTimer != null)
            {
                _countdownTimer.Stop();
                _countdownTimer.Close();
                _countdownTimer = null;
                ShareTimerRunning = false;
            }          
        }
    }
}
