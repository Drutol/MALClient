using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MalClient.Shared.NavArgs
{
    public enum MessageDetailsWorkMode
    {
        Message,
        ProfileComments,
    }

    public class MalMessageDetailsNavArgs
    {
        public MessageDetailsWorkMode WorkMode { get; set; }
        public object Arg { get; set; } //either comment or message
    }
}
