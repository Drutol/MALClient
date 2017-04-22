using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MALClient.Adapters;
using Newtonsoft.Json;

namespace MALClient.WPF.Adapters
{
    public class DataCache : IDataCache
    {
        Dictionary<string, string> _dummyDataStorage = new Dictionary<string, string>();

        public async Task SaveData<T>(T data, string filename, string targetFolder)
        {
            if (_dummyDataStorage.ContainsKey($"{targetFolder}/{filename}"))
                _dummyDataStorage[$"{targetFolder}/{filename}"] = JsonConvert.SerializeObject(data);
            else
                _dummyDataStorage.Add($"{targetFolder}/{filename}", JsonConvert.SerializeObject(data));
        }

        public async Task<T> RetrieveData<T>(string filename, string originFolder, int expiration)
        {
            if (_dummyDataStorage.ContainsKey($"{originFolder}/{filename}"))
                return JsonConvert.DeserializeObject<T>(_dummyDataStorage[$"{originFolder}/{filename}"]);
            return default(T);
        }

        public async Task SaveDataRoaming<T>(T data, string filename)
        {
            if (_dummyDataStorage.ContainsKey($"{filename}"))
                _dummyDataStorage[$"{filename}"] = JsonConvert.SerializeObject(data);
            else
                _dummyDataStorage.Add($"{filename}", JsonConvert.SerializeObject(data));
        }

        public async Task<T> RetrieveDataRoaming<T>(string filename, int expiration)
        {
            if (_dummyDataStorage.ContainsKey(filename))
                return JsonConvert.DeserializeObject<T>(_dummyDataStorage[filename]);
            return default(T);
        }

        public async Task ClearApiRelatedCache()
        {

        }

        public async Task ClearAnimeListData()
        {

        }

    }

}