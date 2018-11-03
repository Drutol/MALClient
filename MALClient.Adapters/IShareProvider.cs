using System;
using System.Collections.Generic;
using System.Text;

namespace MALClient.Adapters
{
    public interface IShareProvider
    {
        void Share(string message);
    }
}
