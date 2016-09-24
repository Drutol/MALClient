using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Models.Models.Misc
{
    public class ExactAiringTimeData
    {
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan Time { get; set; }
    }
}
