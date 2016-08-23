using System.Windows.Input;

namespace MALClient.Models.Models.Misc
{
    public class CachedEntryModel
    {
        public string FileName { get; set; }
        public string Date { get; set; }
        public string Size { get; set; }

        //TODO Xamarin
        //public ICommand RemoveFileCommand => new RelayCommand(async () =>
        //{
        //    var file = await ApplicationData.Current.LocalFolder.GetFileAsync(FileName);
        //    await file.DeleteAsync();
        //});
    }
}
