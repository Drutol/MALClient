using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Net;
using Android.Provider;
using Android.Widget;
using GalaSoft.MvvmLight.Ioc;
using MALClient.Adapters;
using Environment = Android.OS.Environment;
using File = Java.IO.File;
using Uri = Android.Net.Uri;

namespace MALClient.Android.Adapters
{
    public class ImageDownloaderService : IImageDownloaderService
    {
        public const int RequestCodeOpenDirectory = 1;

        public async void DownloadImage(string url, File target)
        {
            if (url == null)
                return;
            try
            {
                var http = new HttpClient();
                byte[] response = { };

                //get bytes
                await Task.Run(async () => response = await http.GetByteArrayAsync(url));

                System.IO.File.WriteAllBytes(target.AbsolutePath,response);


                var toast = Toast.MakeText(SimpleIoc.Default.GetInstance<Activity>(),"File saved successfully.",ToastLength.Short);
                toast.Show();
            }
            catch (Exception)
            {
                var toast = Toast.MakeText(SimpleIoc.Default.GetInstance<Activity>(), "Error. File didn't save properly.", ToastLength.Short);
                toast.Show();
            }
        }

        public void DownloadImage(string url, string suggestedFilename, bool animeConver)
        {
            DownloadImageDefault(url,suggestedFilename,animeConver);
        }

        public void DownloadImageDefault(string url, string suggestedFilename, bool animeCover)
        {
            try
            {
                var img = CreateImageFile(suggestedFilename);
                DownloadImage(url,img);
                UpdateGallery(img);
            }
            catch (Exception)
            {
                var toast = Toast.MakeText(SimpleIoc.Default.GetInstance<Activity>(), "Error. File didn't save properly.", ToastLength.Short);
                toast.Show();
            }
        }

        public static File CreateImageFile(string name)
        {
            // Create an image file name
            File storageDir = new File(Environment.GetExternalStoragePublicDirectory(Environment.DirectoryPictures) +"/MALClient/");
            var chars = Path.GetInvalidFileNameChars();
            var cleanName = name.Aggregate("", (s, c) =>
            {
                if (!chars.Contains(c))
                    s += c;
                return s;
            });
            cleanName += ".png";
            File image = new File(storageDir,cleanName);
            if (!storageDir.Exists())
                storageDir.Mkdirs();
            return image;
        }

        private void UpdateGallery(File file)
        {
            Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
            Uri contentUri = Uri.FromFile(file);
            mediaScanIntent.SetData(contentUri);
            Application.Context.SendBroadcast(mediaScanIntent);
        }
    }
}
