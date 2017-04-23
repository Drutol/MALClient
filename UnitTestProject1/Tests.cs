using System;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using MALClient.Adapters;
using MALClient.Adapters.Credentials;
using MALClient.Models.Enums;
using MALClient.Models.Models.Forums;
using MALClient.WPF;
using MALClient.WPF.Adapters;
using MALClient.XShared.Comm;
using MALClient.XShared.Comm.MagicalRawQueries.Forums;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using CollectionAssert = NUnit.Framework.CollectionAssert;
using DataCache = MALClient.WPF.Adapters.DataCache;

namespace UnitTestProject1
{
    [TestClass]
    public class Tests
    {
        public Tests()
        {
            ViewModelLocator.RegisterBase();
            ResourceLocator.RegisterBase();
            SimpleIoc.Default.Register<IDataCache, DataCache>();
            SimpleIoc.Default.Register<IPasswordVault, PasswordVaultProvider>();
            SimpleIoc.Default.Register<IApplicationDataService, ApplicationDataServiceService>();
            SimpleIoc.Default.Register<IDatabaseContextProvider, DatabaseContextProvider>();

            Credentials.Init();
        }

        [TestCase]
        public async Task TestDatabaseCache()
        {
            var dbProvider = ResourceLocator.DatabaseService;

            var library = await new LibraryListQuery("MALClientTestAcc", AnimeListWorkModes.Anime).GetLibrary(true);
            await Task.Delay(2000);
            var cached = await dbProvider.RetrieveShowsForUser("MALClientTestAcc", true);

            foreach (var libraryData in cached)
            {
                if (!library.Any(data => data.Id == libraryData.Id))
                    Assert.Fail();
            }
        }

        [TestCase]
        public async Task TestSerialization()
        {
            var posts = await ForumTopicQueries.GetTopicData("1499207", 1);
            var serialized = new MyFancyDataSerializer().SerializeObject(posts, typeof(ForumTopicData));
            var deserialized = new MyFancyDataSerializer().DeserializeObject(serialized, typeof(ForumTopicData)) as ForumTopicData;

            Assert.True(deserialized.Equals(posts));
        }
    }
}
