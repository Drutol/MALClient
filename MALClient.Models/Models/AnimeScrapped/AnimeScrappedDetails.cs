using System;
using System.Collections.Generic;
using System.Linq;
using MALClient.Models.Models.Misc;
using SQLite;

namespace MALClient.Models.Models.AnimeScrapped
{
    
    public class AnimeScrappedDetails
    {
        [PrimaryKey]
        public int Id { get; set; }
        [Ignore]
        public List<string> AlternativeTitles { get; private set; } = new List<string>();
        [Ignore]
        public List<string> Information { get; private set; } = new List<string>();
        [Ignore]
        public List<string> Statistics { get; private set; } = new List<string>();
        [Ignore]
        public List<string> Openings { get; private set; } = new List<string>();
        [Ignore]
        public List<string> Endings { get; private set; } = new List<string>();

        public string TextBlobAlternativeTitles
        {
            get { return string.Join("||", AlternativeTitles); }
            set { AlternativeTitles = value.Split(new[] {"||"}, StringSplitOptions.RemoveEmptyEntries).ToList(); }
        }

        public string TextBlobInformation
        {
            get { return string.Join("||", Information); }
            set { Information = value.Split(new[] { "||" }, StringSplitOptions.RemoveEmptyEntries).ToList(); }
        }

        public string TextBlobStatistics
        {
            get { return string.Join("||", Statistics); }
            set { Statistics = value.Split(new[] { "||" }, StringSplitOptions.RemoveEmptyEntries).ToList(); }
        }

        public string TextBlobOpenings
        {
            get { return string.Join("||", Openings); }
            set { Openings = value.Split(new[] { "||" }, StringSplitOptions.RemoveEmptyEntries).ToList(); }
        }

        public string TextBlobEndings
        {
            get { return string.Join("||", Endings); }
            set { Endings = value.Split(new[] { "||" }, StringSplitOptions.RemoveEmptyEntries).ToList(); }
        }

        public ExactAiringTimeData ExtractAiringTime(string broadcastLine)
        {
            var parts = broadcastLine.Split(':');

            if (parts[0] == "Broadcast" && parts[1] != "Unknown")
            {
                try
                {
                    var time = new ExactAiringTimeData();
                    var timeParts = string.Join(":", parts.Skip(1)).Split(' ');
                    DayOfWeek day = DayOfWeek.Monday;
                    switch (timeParts[1])
                    {
                        case "Mondays":
                            day = DayOfWeek.Monday;
                            break;
                        case "Tuesdays":
                            day = DayOfWeek.Tuesday;
                            break;
                        case "Wednesdays":
                            day = DayOfWeek.Wednesday;
                            break;
                        case "Thursdays":
                            day = DayOfWeek.Thursday;
                            break;
                        case "Fridays":
                            day = DayOfWeek.Friday;
                            break;
                        case "Saturdays":
                            day = DayOfWeek.Saturday;
                            break;
                        case "Sundays":
                            day = DayOfWeek.Sunday;
                            break;
                    }
                    time.DayOfWeek = day;
                    time.Time = TimeSpan.Parse(timeParts[3]);
                    return time;
                }
                catch (Exception)
                {
                    return null;
                }               
            }
            return null;
        }

        public ExactAiringTimeData ExtractAiringTime()
        {
            var line = Information.FirstOrDefault(s => s.StartsWith("Broadcast"));
            return line == null ? null : ExtractAiringTime(line);
        }
    }
}
