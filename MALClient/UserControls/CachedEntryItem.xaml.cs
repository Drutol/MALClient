using System;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.UserControls
{
    public sealed partial class CachedEntryItem : UserControl
    {

        public CachedEntryItem(StorageFile file, bool nonUser)
        {
            InitializeComponent();
            TxtUser.Text = file.Name; 
            SetDetails(file);
        }

        public string fileName { get; set; }
        public DateTime saveTime { get; set; }

        private async void SetDetails(StorageFile file)
        {
            var data = await file.GetBasicPropertiesAsync();

            saveTime = data.DateModified.LocalDateTime;
            TxtDate.Text = saveTime.ToString("dd/MM/yyyy HH:mm");
            TxtSize.Text = Utils.SizeSuffix((long) data.Size);
        }

        private async void DeleteFile(object sender, RoutedEventArgs e)
        {
            var file = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
            await file.DeleteAsync();
            IsEnabled = false;
            Background = new SolidColorBrush(Colors.DarkGray);
        }
    }
}