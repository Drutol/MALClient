using System.IO;
using MALClient.Adapters;
using SQLite;

namespace MALClient.WPF.Adapters
{
    public class DatabaseContextProvider : IDatabaseContextProvider
    {
        public SQLiteAsyncConnection GetAsyncConnection()
        {
            return new SQLiteAsyncConnection(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "MALClientDb.db3"));
        }

        public SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "MALClientDb.db3"));
        }
    }
}
