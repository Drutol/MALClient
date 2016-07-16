using System;
using System.Net;
using System.Text;
using MalClient.Shared.Models.Library;
using MalClient.Shared.Utils;

namespace MalClient.Shared.Comm.Manga
{
    public class MangaUpdateQuery : Query
    {
        public static bool UpdatedSomething; //used for data saving on suspending in app.xaml.cs

        public MangaUpdateQuery(IAnimeData item)
            : this(
                item.Id, item.MyEpisodes, item.MyStatus, (int) item.MyScore, item.MyVolumes, item.StartDate,
                item.EndDate,item.Notes)
        {
        }


        private MangaUpdateQuery(int id, int watchedEps, int myStatus, int myScore, int myVol, string startDate,
            string endDate,string notes)
        {
            UpdatedSomething = true;
            if (startDate != null)
            {
                var splitDate = startDate.Split('-');
                startDate = $"{splitDate[1]}{splitDate[2]}{splitDate[0]}";
            }
            if (endDate != null)
            {
                var splitDate = endDate.Split('-');
                endDate = $"{splitDate[1]}{splitDate[2]}{splitDate[0]}";
            }
            var xml = new StringBuilder();
            xml.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xml.AppendLine("<entry>");
            xml.AppendLine($"<chapter>{watchedEps}</chapter>");
            xml.AppendLine($"<status>{myStatus}</status>");
            xml.AppendLine($"<score>{myScore}</score>");
            xml.AppendLine($"<volume>{myVol}</volume>");
            //xml.AppendLine("<storage_type></storage_type>");
            //xml.AppendLine("<storage_value></storage_value>");
            //xml.AppendLine("<times_rewatched></times_rewatched>");
            //xml.AppendLine("<rewatch_value></rewatch_value>");
            if(startDate != null) xml.AppendLine($"<date_start>{startDate}</date_start>");
            if(endDate != null) xml.AppendLine($"<date_finish>{endDate}</date_finish>");
            //xml.AppendLine("<priority></priority>");
            //xml.AppendLine("<enable_discussion></enable_discussion>");
            //xml.AppendLine("<enable_rewatching></enable_rewatching>");
            //xml.AppendLine("<comments></comments>");
            //xml.AppendLine("<fansub_group></fansub_group>");
            xml.AppendLine($"<tags>{notes}</tags>");
            xml.AppendLine("</entry>");


            Request =
                WebRequest.Create(Uri.EscapeUriString($"http://myanimelist.net/api/mangalist/update/{id}.xml?data={xml}"));
            Request.Credentials = Credentials.GetHttpCreditentials();
            Request.ContentType = "application/x-www-form-urlencoded";
            Request.Method = "GET";
        }
    }
}