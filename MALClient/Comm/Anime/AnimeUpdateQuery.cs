using System;
using System.Net;
using System.Text;
using MALClient.Items;

namespace MALClient.Comm
{
    internal class AnimeUpdateQuery : Query
    {
        public AnimeUpdateQuery(IAnimeData item) : this(item.Id, item.MyEpisodes, item.MyStatus, item.MyScore,item.StartDate,item.EndDate)
        {
        }


        public AnimeUpdateQuery(int id, int watchedEps, int myStatus, int myScore,string startDate,string endDate)
        {
            switch (CurrentApiType)
            {
                case ApiType.Mal:
                    UpdateAnimeMal(id,watchedEps,myStatus,myScore,startDate,endDate);
                    break;
                case ApiType.Hummingbird:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateAnimeMal(int id, int watchedEps, int myStatus, int myScore, string startDate, string endDate)
        {
            var splitDate = startDate.Split('-');
            startDate = $"{splitDate[1]}{splitDate[2]}{splitDate[0]}";
            splitDate = endDate.Split('-');
            endDate = $"{splitDate[1]}{splitDate[2]}{splitDate[0]}"; //mmddyyyy
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
            xml.AppendLine($"<date_start>{startDate}</date_start>");
            xml.AppendLine($"<date_finish>{endDate}</date_finish>");
            //xml.AppendLine("<priority></priority>");
            //xml.AppendLine("<enable_discussion></enable_discussion>");
            //xml.AppendLine("<enable_rewatching></enable_rewatching>");
            //xml.AppendLine("<comments></comments>");
            //xml.AppendLine("<fansub_group></fansub_group>");
            //xml.AppendLine("<tags></tags>");
            xml.AppendLine("</entry>");


            Request = WebRequest.Create(Uri.EscapeUriString($"http://myanimelist.net/api/animelist/update/{id}.xml?data={xml}"));
            Request.Credentials = Credentials.GetHttpCreditentials();
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }

        private void UpdateAnimeHummingbird()
        {
        }
    }
}