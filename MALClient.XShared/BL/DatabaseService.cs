using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MALClient.Adapters;
using MALClient.Models.Enums;
using MALClient.Models.Models.AnimeScrapped;
using MALClient.Models.Models.Library;
using SQLite;

namespace MALClient.XShared.BL
{
    public class DatabaseService
    {
        private readonly IDatabaseContextProvider _contextProvider;

        public DatabaseService(IDatabaseContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
            AsyncConnection = _contextProvider.GetAsyncConnection();
            SyncConnection = _contextProvider.GetConnection();
        }

        private SQLiteAsyncConnection AsyncConnection { get; set; }
        private SQLiteConnection SyncConnection { get; set; }


        public async Task<AnimeScrappedDetails> RetrieveAnimeDetails(int id)
        {
           return await (await GetOrCreateTable<AnimeScrappedDetails>()).Where(details => details.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<ILibraryData>> RetrieveShowsForUser(string username, bool anime)
        {
            if (anime)
                return
                    (await (await GetOrCreateTable<AnimeLibraryItemData>()).Where(
                        entry =>
                           username == entry.Owner).ToListAsync())
                    .Cast<ILibraryData>().ToList();
            return
                (await (await GetOrCreateTable<MangaLibraryItemData>()).Where(
                    entry =>
                        username == entry.Owner).ToListAsync())
                .Cast<ILibraryData>().ToList();
        }

        public async Task<List<TopAnimeData>> RetrieveTopAnimeData(TopAnimeType type)
        {
            return
                await (await GetOrCreateTable<TopAnimeData>()).Where(
                    entry =>
                        entry.Type == type).ToListAsync();
        }

        public async void SaveLibraryDetails(ILibraryData data)
        {
            if (data is AnimeLibraryItemData)
            {
                if (!IsTableAvailable(typeof(AnimeLibraryItemData)))
                    await AsyncConnection.CreateTableAsync<AnimeLibraryItemData>();
            }
            else
            {
                if (!IsTableAvailable(typeof(MangaLibraryItemData)))
                    await AsyncConnection.CreateTableAsync<MangaLibraryItemData>();
            }

            await AsyncConnection.InsertOrReplaceAsync(data);
        }

        public async void SaveTopAnime(IEnumerable<TopAnimeData> data)
        {
            if (!IsTableAvailable(typeof(TopAnimeData)))
                await AsyncConnection.CreateTableAsync<TopAnimeData>();

            foreach (var topAnimeData in data)
            {
                await AsyncConnection.InsertOrReplaceAsync(topAnimeData);
            }
        }

        public async void SaveLibraryDetails(List<ILibraryData> data)
        {
            if(!data.Any())
                return;

            if (data.First() is AnimeLibraryItemData)
            {
                if (!IsTableAvailable(typeof(AnimeLibraryItemData)))
                    await AsyncConnection.CreateTableAsync<AnimeLibraryItemData>();
            }
            else
            {
                if (!IsTableAvailable(typeof(MangaLibraryItemData)))
                    await AsyncConnection.CreateTableAsync<MangaLibraryItemData>();
            }

            foreach (var libraryData in data)
            {
                await AsyncConnection.InsertOrReplaceAsync(libraryData);
            }

        }

        public async void SaveAnimeDetails(AnimeScrappedDetails data)
        {
            if (!IsTableAvailable(typeof(AnimeScrappedDetails)))
                await AsyncConnection.CreateTableAsync<AnimeScrappedDetails>();

            await AsyncConnection.InsertOrReplaceAsync(data);
        }

        private async Task<AsyncTableQuery<T>> GetOrCreateTable<T>() where T : new()
        {
            if (!IsTableAvailable(typeof(T)))
                await AsyncConnection.CreateTableAsync<T>();

            return AsyncConnection.Table<T>();
        }

        private bool IsTableAvailable(Type type)
        {
            return IsTableAvailable(type.Name);
        }

        private bool IsTableAvailable(string type)
        {
            var tableInfo = SyncConnection.GetTableInfo(type);
            return tableInfo.Any();
        }
    }
}
