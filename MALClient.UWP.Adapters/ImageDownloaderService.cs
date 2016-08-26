using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using MalClient.Shared;
using MALClient.Adapters;

namespace MALClient.UWP.Adapters
{
    public class ImageDownloaderService : IImageDownloaderService
    {
        public async void DownloadImage(string url, string suggestedFilename = "")
        {
            if (url == null)
                return;
            try
            {
                var sp = new FileSavePicker();
                sp.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                sp.FileTypeChoices.Add("Portable Network Graphics (*.png)", new List<string> {".png"});
                sp.SuggestedFileName = $"{suggestedFilename}-cover_art";

                var file = await sp.PickSaveFileAsync();
                if (file == null)
                    return;
                var http = new HttpClient();
                byte[] response = {};

                //get bytes
                await Task.Run(async () => response = await http.GetByteArrayAsync(url));


                var fs = await file.OpenStreamForWriteAsync(); //get stream
                var writer = new DataWriter(fs.AsOutputStream());

                writer.WriteBytes(response); //write
                await writer.StoreAsync();
                await writer.FlushAsync();

                writer.Dispose();
                UWPUtilities.GiveStatusBarFeedback("File saved successfully.");
            }
            catch (Exception)
            {
                UWPUtilities.GiveStatusBarFeedback("Error. File didn't save properly.");
            }
        }
    }
}
