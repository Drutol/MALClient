using System;
using System.Windows.Input;
using Windows.Storage;
using GalaSoft.MvvmLight.Command;

namespace MalClient.Shared.Models.Misc
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
