using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using MALClient.Shared;
using MALClient.Adapters;

namespace MALClient.UWP.Adapters
{
    public class ImageDownloaderService : IImageDownloaderService
    {
        public async void DownloadImage(string url, string suggestedFilename,bool animeCover)
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
                await Download(url, file,animeCover);
                UWPUtilities.GiveStatusBarFeedback("File saved successfully.");
            }
            catch (Exception)
            {
                UWPUtilities.GiveStatusBarFeedback("Error. File didn't save properly.");
            }
        }

        public async void DownloadImageDefault(string url, string suggestedFilename,bool animeCover)
        {
            try
            {
                var lib = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Pictures);
                var folder = await lib.SaveFolder.CreateFolderAsync("MALCLient Images",CreationCollisionOption.OpenIfExists);
                var file = await folder.CreateFileAsync(UWPUtilities.SanitizeFileName(suggestedFilename) + ".png");
                await Download(url, file,animeCover);
                UWPUtilities.GiveStatusBarFeedback("File saved successfully.");
            }
            catch (Exception)
            {
                UWPUtilities.GiveStatusBarFeedback("Error. File didn't save properly.");
            }
        }

        private async Task Download(string url,StorageFile file,bool cover)
        {
            var http = new HttpClient();
            byte[] response = { };
            string betterUrl = url;
            if(cover)
            {
                var pos = betterUrl.IndexOf(".jpg");
                if (pos != -1)
                    betterUrl = betterUrl.Insert(pos, "l");
            }

            //get bytes
            try
            {
                await Task.Run(async () => response = await http.GetByteArrayAsync(betterUrl));
            }
            catch (Exception)
            {
                await Task.Run(async () => response = await http.GetByteArrayAsync(url));
            }

            var fs = await file.OpenStreamForWriteAsync(); //get stream
            var writer = new DataWriter(fs.AsOutputStream());

            writer.WriteBytes(response); //write
            await writer.StoreAsync();
            await writer.FlushAsync();

            writer.Dispose();
        }
    }
}
