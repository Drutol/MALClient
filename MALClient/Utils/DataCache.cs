using System;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace MALClient
{
    public static class DataCache
    {
        public static async void SaveDataForUser(string user, string data)
        {
            if(!Utils.IsCachingEnabled())
                return;
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync($"anime_data_{user.ToLower()}.xml",CreationCollisionOption.ReplaceExisting);
            var builder = new StringBuilder(data);
            builder.AppendLine(Utils.ConvertToUnixTimestamp(DateTime.Now).ToString());
            data = builder.ToString();
            await FileIO.WriteTextAsync(file, data);
        }

        public static async Task<Tuple<string,DateTime>> RetrieveDataForUser(string user)
        {
            if (!Utils.IsCachingEnabled())
                return null;
            try
            {
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync($"anime_data_{user.ToLower()}.xml");
                var data = await Windows.Storage.FileIO.ReadTextAsync(file);
                var lines = data.Split(new char[] {'\n', '\r'},StringSplitOptions.RemoveEmptyEntries);
                DateTime lastUpdateTime = new DateTime();
                if (!CheckForOldData(lines[lines.Length - 1],ref lastUpdateTime))
                {
                    await file.DeleteAsync();
                    return null;
                }
                data = "";
                for (int i = 0; i < lines.Length - 1; i++)
                {
                    data += lines[i];
                }
                return new Tuple<string, DateTime>(data,lastUpdateTime);
            }
            catch (Exception)
            {
                return null;
            }            
        }

        private static bool CheckForOldData(string timestamp, ref DateTime time)
        {
            var data = Utils.ConvertFromUnixTimestamp(double.Parse(timestamp));
            TimeSpan diff = DateTime.Now.ToUniversalTime().Subtract(data);
            time = data;
            if (diff.TotalSeconds > Utils.GetCachePersitence())
                return false;
            return true;
        }
    }
}
