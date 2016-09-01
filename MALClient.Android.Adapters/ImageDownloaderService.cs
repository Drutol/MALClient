using MALClient.Adapters;

namespace MALClient.Android.Adapters
{
    public class ImageDownloaderService : IImageDownloaderService
    {
        public async void DownloadImage(string url, string suggestedFilename = "")
        {
            //if (url == null)
            //    return;
            //try
            //{
            //    var sp = new FileSavePicker();
            //    sp.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            //    sp.FileTypeChoices.Add("Portable Network Graphics (*.png)", new List<string> {".png"});
            //    sp.SuggestedFileName = $"{suggestedFilename}-cover_art";

            //    var file = await sp.PickSaveFileAsync();
            //    if (file == null)
            //        return;
            //    var http = new HttpClient();
            //    byte[] response = {};

            //    //get bytes
            //    await Task.Run(async () => response = await http.GetByteArrayAsync(url));


            //    var fs = await file.OpenStreamForWriteAsync(); //get stream
            //    var writer = new DataWriter(fs.AsOutputStream());

            //    writer.WriteBytes(response); //write
            //    await writer.StoreAsync();
            //    await writer.FlushAsync();

            //    writer.Dispose();
            //    UWPUtilities.GiveStatusBarFeedback("File saved successfully.");
            //}
            //catch (Exception)
            //{
            //    UWPUtilities.GiveStatusBarFeedback("Error. File didn't save properly.");
            //}
        }
    }
}
