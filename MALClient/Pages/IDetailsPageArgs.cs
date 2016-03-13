using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Pages
{
    public interface IDetailsPageArgs
    {
        int Id { get; set; }
        string Title { get; set; }
    }
}
