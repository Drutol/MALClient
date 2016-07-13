using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml.Media;
using GalaSoft.MvvmLight.Command;

namespace MALClient.Models
{
    public class CachedEntryModel
    {
        public string FileName { get; set; }
        public string Date { get; set; }
        public string Size { get; set; }

        public ICommand RemoveFileCommand => new RelayCommand(async () =>
        {
            var file = await ApplicationData.Current.LocalFolder.GetFileAsync(FileName);
            await file.DeleteAsync();
        });
    }
}
