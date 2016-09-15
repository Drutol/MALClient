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
			throw new NotImplementedException();
		}

		public Task ClearApiRelatedCache()
		{
			throw new NotImplementedException();
		}

		public Task<T> RetrieveData<T>(string filename, string originFolder, int expiration)
		{
			throw new NotImplementedException();
		}

		public Task<T> RetrieveDataRoaming<T>(string filename, int expiration)
		{
			throw new NotImplementedException();
		}

		public Task SaveData<T>(T data, string filename, string targetFolder)
		{
			throw new NotImplementedException();
		}

		public Task SaveDataRoaming<T>(T data, string filename)
		{
			throw new NotImplementedException();
		}
	}
}