using System;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using MALClient.XShared.Utils;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.Shared.UserControls
{
    public sealed partial class CachedEntryItem : UserControl
    {
        public CachedEntryItem(StorageFile file, bool nonUser)
        {
            InitializeComponent();
            TxtUser.Text = file.Name;
            SetDetails(file);
        }

        private string _fileName { get; set; }
        private DateTime _saveTime { get; set; }

        private async void SetDetails(StorageFile file)
        {
            var data = await file.GetBasicPropertiesAsync();
            _fileName = file.Name;
            _saveTime = data.DateModified.LocalDateTime;
            TxtDate.Text = _saveTime.ToString("dd/MM/yyyy HH:mm");
            TxtSize.Text = Utilities.SizeSuffix((long) data.Size);
        }

        private async void DeleteFile(object sender, RoutedEventArgs e)
        {
            var file = await ApplicationData.Current.LocalFolder.GetFileAsync(_fileName);
            await file.DeleteAsync();
            IsEnabled = false;
            Background = new SolidColorBrush(Colors.DarkGray);
        }
    }
}