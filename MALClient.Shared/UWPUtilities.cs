using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Appointments;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.ViewManagement;
using MALClient.Models.Enums;
using MALClient.XShared.Comm.Profile;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;

namespace MALClient.UWP.Shared
{
    public static class UWPUtilities
    {
        public static AppointmentDaysOfWeek DayToAppointementDay(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Friday:
                    return AppointmentDaysOfWeek.Friday;
                case DayOfWeek.Monday:
                    return AppointmentDaysOfWeek.Monday;
                case DayOfWeek.Saturday:
                    return AppointmentDaysOfWeek.Saturday;
                case DayOfWeek.Sunday:
                    return AppointmentDaysOfWeek.Sunday;
                case DayOfWeek.Thursday:
                    return AppointmentDaysOfWeek.Thursday;
                case DayOfWeek.Tuesday:
                    return AppointmentDaysOfWeek.Tuesday;
                case DayOfWeek.Wednesday:
                    return AppointmentDaysOfWeek.Wednesday;
                default:
                    throw new ArgumentOutOfRangeException(nameof(day), day, null);
            }
        }    

        /// <summary>
        ///     http://stackoverflow.com/questions/28635208/retrieve-the-current-app-version-from-package
        /// </summary>
        /// <returns></returns>
        public static string GetAppVersion()
        {
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }

        public static async Task RemoveProfileImg()
        {
            try
            {
                await (await ApplicationData.Current.LocalFolder.GetFileAsync("UserImg.png")).DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
            catch (Exception)
            {
                //no file
            }
        }

        public static async Task DownloadProfileImg()
        {
            if (!Credentials.Authenticated)
                return;
            try
            {
                var folder = ApplicationData.Current.LocalFolder;
                var thumb = await folder.CreateFileAsync("UserImg.png", CreationCollisionOption.ReplaceExisting);

                var http = new HttpClient();
                byte[] response = { };
                switch (Settings.SelectedApiType)
                {
                    case ApiType.Mal:
                        await Task.Run(async () => response = await http.GetByteArrayAsync($"https://cdn.myanimelist.net/images/userimages/{Credentials.Id}.jpg"));
                        break;
                    case ApiType.Hummingbird:
                        var avatarLink = await new ProfileQuery().GetHummingBirdAvatarUrl();
                        await Task.Run(async () => response = await http.GetByteArrayAsync(avatarLink));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                //get bytes

                var fs = await thumb.OpenStreamForWriteAsync(); //get stream
                var writer = new DataWriter(fs.AsOutputStream());

                writer.WriteBytes(response); //write
                await writer.StoreAsync();
                await writer.FlushAsync();

                writer.Dispose();

                await ViewModelLocator.GeneralHamburger.UpdateProfileImg(false);
            }
            catch (Exception)
            {
                //
            }
            await Task.Delay(2000);
            await ViewModelLocator.GeneralHamburger.UpdateProfileImg(false);
        }

       
      public static async void GiveStatusBarFeedback(string text)
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                try
                {
                    var sb = StatusBar.GetForCurrentView().ProgressIndicator;
                    sb.Text = text;
                    sb.ProgressValue = null;
                    await sb.ShowAsync();
                    await Task.Delay(2000);
                    await sb.HideAsync();
                }
                catch (Exception)
                {
                    //
                }
            }
        }

        public static string SanitizeFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }
    }
}
