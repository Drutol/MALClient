using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using MALClient.Adapters;
using SQLite;

namespace MALClient.UWP.Adapters
{
    public class DatabaseContextProvider : IDatabaseContextProvider
    {
        public SQLiteAsyncConnection GetAsyncConnection()
        {
            return new SQLiteAsyncConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, "MALClientDb.db3"));
        }

        public SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, "MALClientDb.db3"));
        }
    }
}
