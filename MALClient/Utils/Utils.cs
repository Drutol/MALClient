using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Utils
{
    public static class Utils
    {
        public static string StatusToString(int status)
        {
            switch (status)
            {
                case 1:
                    return "Watching";
                case 2:
                    return "Completed";
                case 3:
                    return "On hold";
                case 4:
                    return "Dropped";
                case 6:
                    return "Plan to watch";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
