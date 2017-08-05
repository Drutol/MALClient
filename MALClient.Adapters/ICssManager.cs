using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Adapters
{
    public interface ICssManager
    {
        string WrapWithCss(string html, bool withImgCss = true, int? minWidth = null);
    }
}
