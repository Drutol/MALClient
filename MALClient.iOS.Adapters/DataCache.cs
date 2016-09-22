using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MALClient.Adapters;
using Newtonsoft.Json;

namespace MALClient.iOS.Adapters
{
	public class DataCache : IDataCache
	{
		static bool CheckForOldData(DateTime date, int days = 7)
		{
			var diff = DateTime.Now.ToUniversalTime().Subtract(date);
			if (diff.TotalDays >= days)
				return false;
			return true;
		}
		public Task ClearAnimeListData()
		{
			return Task.Run(() =>
			{
				var targetFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
				var files = Directory.GetFiles(targetFolder);

				foreach (var item in files.Where((string arg) => arg.Contains("_data_")))
				{
					File.Delete(item);
				}
			});
		}

		public Task ClearApiRelatedCache()
		{
			return Task.Delay(10);
			//throw new NotImplementedException();
		}

		public Task<T> RetrieveData<T>(string filename, string originFolder, int expiration)
		{
			return Task.Run(() =>
			{
				var specialfolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
				var filepath = Path.Combine(originFolder, filename);
				if (!File.Exists(filepath))
				{
					File.Create(filepath).Dispose();
				}
				var data = File.ReadAllText(filepath);

				var returnData = JsonConvert.DeserializeObject<Tuple<DateTime, T>>(data);

				return expiration > 1
						? CheckForOldData(returnData.Item1, expiration) ? returnData.Item2 : default(T)
						: returnData.Item2;
			});
		}

		public Task<T> RetrieveDataRoaming<T>(string filename, int expiration)
		{
			return RetrieveData<T>(filename, "roaming", expiration);
		}

		public Task SaveData<T>(T data, string filename, string targetFolder)
		{
			return Task.Run(() =>
			{
				var specialFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
				var file = Path.Combine(specialFolder, targetFolder, filename);

				var json = JsonConvert.SerializeObject(new Tuple<DateTime,T>(DateTime.UtcNow,data));

				File.WriteAllText(file, json);
			}); 
		}

		public Task SaveDataRoaming<T>(T data, string filename)
		{
			return SaveData(data, filename, "roaming");
		}
	}
}