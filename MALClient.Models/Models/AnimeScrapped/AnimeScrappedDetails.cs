using System;
using System.Collections.Generic;
using System.Linq;
using MALClient.Models.Models.Misc;

namespace MALClient.Models.Models.AnimeScrapped
{
    public class AnimeScrappedDetails
    {
        public int Id { get; set; }
        public List<string> AlternativeTitles { get; } = new List<string>();
        public List<string> Information { get; } = new List<string>();
        public List<string> Statistics { get; } = new List<string>();
        public List<string> Openings { get; } = new List<string>();
        public List<string> Endings { get; } = new List<string>();

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
