using System;
using System.Net;
using System.Text;
using MalClient.Shared.Models.Library;
using MalClient.Shared.Utils;
using MalClient.Shared.Utils.Enums;
using MalClient.Shared.Utils.Managers;

namespace MalClient.Shared.Comm.Anime
{
    public class AnimeUpdateQuery : Query
    {
        public static bool UpdatedSomething; //used for data saving on suspending in app.xaml.cs

        public AnimeUpdateQuery(IAnimeData item)
: this(item.Id, item.MyEpisodes, item.MyStatus, item.MyScore, item.StartDate, item.EndDate, item.Notes)
        {
            LiveTilesManager.UpdateTile(item);
        }


        private AnimeUpdateQuery(int id, int watchedEps, int myStatus, float myScore, string startDate, string endDate, string notes)
        {
            UpdatedSomething = true;
            switch (CurrentApiType)
            {
                case ApiType.Mal:
                    UpdateAnimeMal(id, watchedEps, myStatus, (int)myScore, startDate, endDate, notes);
                    break;
                case ApiType.Hummingbird:
                    UpdateAnimeHummingbird(id, watchedEps, myStatus, myScore, startDate, endDate);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateAnimeMal(int id, int watchedEps, int myStatus, int myScore, string startDate, string endDate, string notes)
        {
            if (startDate != null)
            {
                var splitDate = startDate.Split('-');
                startDate = $"{splitDate[1]}{splitDate[2]}{splitDate[0]}";
            }
            if (endDate != null)
            {
                var splitDate = endDate.Split('-');
                endDate = $"{splitDate[1]}{splitDate[2]}{splitDate[0]}";
            }//mmddyyyy
            var xml = new StringBuilder();
            xml.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xml.AppendLine("<entry>");
            xml.AppendLine($"<episode>{watchedEps}</episode>");
            xml.AppendLine($"<status>{myStatus}</status>");
            xml.AppendLine($"<score>{myScore}</score>");
            //xml.AppendLine("<download_episodes></download_episodes>");
            //xml.AppendLine("<storage_type></storage_type>");
            //xml.AppendLine("<storage_value></storage_value>");
            //xml.AppendLine("<times_rewatched></times_rewatched>");
            //xml.AppendLine("<rewatch_value></rewatch_value>");
            if (startDate != null) xml.AppendLine($"<date_start>{startDate}</date_start>");
            if (endDate != null) xml.AppendLine($"<date_finish>{endDate}</date_finish>");
            //xml.AppendLine("<priority></priority>");
            //xml.AppendLine("<enable_discussion></enable_discussion>");
            //xml.AppendLine("<enable_rewatching></enable_rewatching>");
            //xml.AppendLine("<comments></comments>");
            //xml.AppendLine("<fansub_group></fansub_group>");
            xml.AppendLine($"<tags>{notes}</tags>");
            xml.AppendLine("</entry>");


            Request =
                WebRequest.Create(Uri.EscapeUriString($"https://myanimelist.net/api/animelist/update/{id}.xml?data={xml}"));
            Request.Credentials = Credentials.GetHttpCreditentials();
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }

        private void UpdateAnimeHummingbird(int id, int watchedEps, int myStatus, float myScore, string startDate,
            string endDate)
        {
            Request =
                WebRequest.Create(
                    Uri.EscapeUriString(
                        $"https://hummingbird.me/api/v1/libraries/{id}?auth_token={Credentials.HummingbirdToken}&episodes_watched={watchedEps}&rating={myScore}&status={AnimeStatusToHum((AnimeStatus) myStatus)}"));
            Request.ContentType = "application/x-www-form-urlencoded";
            {
                Request.Method = "POST";
            }
        }

        private static string AnimeStatusToHum(AnimeStatus status)
        {
            switch (status)
            {
                case AnimeStatus.Watching:
                    return "currently-watching";
                case AnimeStatus.PlanToWatch:
                    return "plan-to-watch";
                case AnimeStatus.Completed:
                    return "completed";
                case AnimeStatus.OnHold:
                    return "on-hold";
                case AnimeStatus.Dropped:
                    return "dropped";
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }
    }
}