using System;
using System.IO;
using System.Threading.Tasks;
using MALClient.Adapters;

namespace MALClient.iOS.Adapters
{
	public class DataCache : IDataCache
	{
		public Task ClearAnimeListData()
		{
			return default(Task);
			//throw new NotImplementedException();
		}

		public Task ClearApiRelatedCache()
		{
			return Task.Delay(10);
			//throw new NotImplementedException();
		}

		public Task<T> RetrieveData<T>(string filename, string originFolder, int expiration)
		{
			return default(Task<T>);
			//throw new NotImplementedException();
		}

		public Task<T> RetrieveDataRoaming<T>(string filename, int expiration)
		{
			return default(Task<T>);
			//throw new NotImplementedException();
		}

		public Task SaveData<T>(T data, string filename, string targetFolder)
		{
			return default(Task);
			//throw new NotImplementedException();
		}

		public Task SaveDataRoaming<T>(T data, string filename)
		{
			return default(Task);
			//throw new NotImplementedException();
		}
	}
}