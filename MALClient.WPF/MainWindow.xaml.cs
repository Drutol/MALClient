using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MALClient.Models.Enums;
using MALClient.Models.Models.Forums;
using MALClient.XShared.BL;
using MALClient.XShared.Comm;
using MALClient.XShared.Comm.Anime;
using MALClient.XShared.Comm.CommUtils;
using MALClient.XShared.Comm.MagicalRawQueries.Forums;
using MALClient.XShared.Utils;
using MALClient.XShared.Utils.Managers;
using MALClient.XShared.ViewModels;
using Newtonsoft.Json;

namespace MALClient.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            WPFViewModelLocator.RegisterBase();
            ResourceLocator.RegisterBase();
            Credentials.Init();
            HtmlClassMgr.Init();
            FavouritesManager.LoadData();
            ResourceLocator.HandyDataStorage.Init();
            InitializeComponent();

            TestRun();
        }

        private async void TestRun()
        {
            //var data = await new LibraryListQuery(Credentials.UserName, AnimeListWorkModes.Anime).GetLibrary();

            var posts = await ForumTopicQueries.GetTopicData("1499207", 1);
            var serialized = new MyFancyDataSerializer().SerializeObject(posts,posts.GetType());
            var deserialized = new MyFancyDataSerializer().DeserializeObject(serialized, typeof(ForumTopicData));
        }
    }
}
