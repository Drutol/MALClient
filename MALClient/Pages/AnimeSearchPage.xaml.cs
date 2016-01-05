using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MALClient.Comm;
using MALClient.Items;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MALClient.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AnimeSearchPage : Page
    {
        private ObservableCollection<AnimeSearchItem> _animeSearchItems =  new ObservableCollection<AnimeSearchItem>();

        public AnimeSearchPage()
        {
            this.InitializeComponent();
        }

        internal async void SubmitQuery(string text)
        {
            SpinnerLoading.Visibility = Visibility.Visible;
            EmptyNotice.Visibility = Visibility.Collapsed;
            _animeSearchItems.Clear();
            string response = "";
            await Task.Run(async () => response = await new AnimeSearchQuery(text).GetRequestResponse());

            try
            {
                XDocument parsedData = XDocument.Parse(response);
                foreach (var item in parsedData.Element("anime").Elements("entry"))
                {
                    _animeSearchItems.Add(new AnimeSearchItem(item));
                }
                Animes.ItemsSource = _animeSearchItems;             
            }
            catch (Exception e)
            {
                EmptyNotice.Visibility = Visibility.Visible;
            }
            SpinnerLoading.Visibility = Visibility.Collapsed;
        }
    }
}
