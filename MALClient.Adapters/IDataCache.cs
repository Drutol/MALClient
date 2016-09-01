using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.Adapters
{
    public interface IDataCache
    {
        /// <summary>
        /// Serializes data and writes it to file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">Data to store</param>
        /// <param name="filename">File name with extension</param>
        /// <param name="targetFolder">Null or string.Empty for root folder</param>
        /// <returns></returns>
        Task SaveData<T>(T data, string filename, string targetFolder);

        Task SaveDataRoaming<T>(T data, string filename);

        /// <summary>
        /// Gets serialized data and deserializes it to specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename">File name with extension</param>
        /// <param name="originFolder">Null or string.Empty for root folder</param>
        /// <param name="expiration">Determine from file header whether data is still usable/valid. Values below/equal 0 mean indefinite.</param>
        /// <returns></returns>
        Task<T> RetrieveData<T>(string filename, string originFolder, int expiration);

        Task<T> RetrieveDataRoaming<T>(string filename, int expiration);

        Task ClearApiRelatedCache();

        Task ClearAnimeListData();
    }
}
