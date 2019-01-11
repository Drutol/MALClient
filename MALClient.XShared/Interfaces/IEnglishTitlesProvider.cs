using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MALClient.XShared.Interfaces
{
    public interface IEnglishTitlesProvider
    {
        bool TryGetEnglishTitleForSeries(int id, bool isAnime, out string title);
        void AddOrUpdate(int id, bool isAnime, string title);
        Task Init();
        Task SaveData();
    }
}
