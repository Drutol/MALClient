using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace MALClient
{
    public static class DataCache
    {
        public static async void SaveDataForUser(string user, string data)
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync($"anime_data_{user.ToLower()}.xml",CreationCollisionOption.ReplaceExisting);
            var builder = new StringBuilder(data);
            builder.AppendLine(ConvertToUnixTimestamp(DateTime.Now).ToString());
            data = builder.ToString();
            await FileIO.WriteTextAsync(file, data);
        }

        public static async Task<Tuple<string,TimeSpan>> RetrieveDataForUser(string user)
        {
            try
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync($"anime_data_{user.ToLower()}.xml");
                var data = await Windows.Storage.FileIO.ReadTextAsync(file);
                var lines = data.Split(new char[] {'\n', '\r'},StringSplitOptions.RemoveEmptyEntries);
                TimeSpan diff;
                if (!CheckForOldData(lines[lines.Length - 1],ref diff))
                {
                    await file.DeleteAsync();
                    return null;
                }
                data = "";
                for (int i = 0; i < lines.Length - 1; i++)
                {
                    data += lines[i];
                }
                return new Tuple<string, TimeSpan>(data,diff);
            }
            catch (Exception)
            {
                return null;
            }            
        }

        public static bool CheckForOldData(string timestamp, ref TimeSpan time)
        {
            var data = ConvertFromUnixTimestamp(double.Parse(timestamp));
            TimeSpan diff = DateTime.Now.ToUniversalTime().Subtract(data);
            time = diff;
            if (diff.TotalSeconds > 3600)
                return false;
            return true;
        }

        private static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }

        private static int ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return (int)Math.Floor(diff.TotalSeconds);
        }
    }
}
