using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MALClient.UserControls
{
    public sealed partial class CachedEntryItem : UserControl
    {
        public string user { get; set; }
        public string fileName { get; set; }
        public DateTime saveTime { get; set; }

        public CachedEntryItem(StorageFile file)
        {
            this.InitializeComponent();
            user = file.DisplayName.Split('_')[2];
            fileName = file.Name;           
            TxtUser.Text = user;
            SetDetails(file);                    
        }

        private async void SetDetails(StorageFile file)
        {
            var data = await file.GetBasicPropertiesAsync();
            
            saveTime = data.DateModified.LocalDateTime;
            TxtDate.Text = saveTime.ToString("dd/MM/yyyy HH:mm");
            TxtSize.Text = Utils.SizeSuffix((long)data.Size);
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
