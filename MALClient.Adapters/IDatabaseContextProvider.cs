using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace MALClient.Adapters
{
    public interface IDatabaseContextProvider
    {
        SQLiteAsyncConnection GetAsyncConnection();
        SQLiteConnection GetConnection();
    }
}
