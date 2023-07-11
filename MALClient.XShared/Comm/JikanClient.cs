using System;
using System.Collections.Generic;
using System.Text;
using JikanDotNet;
using JikanDotNet.Config;

namespace MALClient.XShared.Comm;
public static class JikanClient
{
    public static readonly Jikan Jikan = new Jikan(new JikanClientConfiguration
    {
        SuppressException = true,
        LimiterConfigurations = new List<TaskLimiterConfiguration>
        {
            new TaskLimiterConfiguration(3, TimeSpan.FromSeconds(1)),
            new TaskLimiterConfiguration(60, TimeSpan.FromSeconds(60))
        }
    });
}
