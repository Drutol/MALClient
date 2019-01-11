using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Android.Runtime;
using MALClient.Adapters;
using MALClient.XShared.Interfaces;

namespace MALClient.XShared.BL
{
    [Preserve(AllMembers = true)]
    public class EnglishTitlesProvider : IEnglishTitlesProvider
    {
        private const string StorageFile = "english_titles_cache.json";

        private bool _hasSomethingChanged;

        private readonly IDataCache _cache;
        private Dictionary<string, string> _englishTitles;

        public EnglishTitlesProvider(IDataCache cache)
        {
            _cache = cache;
        }

        public Task Init()
        {
            return LoadData();
        }

        public void AddOrUpdate(int id, bool isAnime, string title)
        {
            if(string.IsNullOrEmpty(title))
                return;

            var sId = FormatId(id, isAnime);

            if (!_englishTitles.ContainsKey(sId) || _englishTitles[sId] != title)
            {
                _englishTitles[sId] = title;
                _hasSomethingChanged = true;
            }         
        }

        private string FormatId(int id, bool isAnime)
        {
            return $"{(isAnime ? "a" : "m")}{id}";
        }

        public bool TryGetEnglishTitleForSeries(int id, bool isAnime, out string title)
        {
            var sId = FormatId(id, isAnime);

            if (_englishTitles.ContainsKey(sId))
            {
                title = _englishTitles[sId];
                return true;
            }

            title = null;
            return false;
        }

        public async Task SaveData()
        {
            if (_hasSomethingChanged)
            {
                await _cache.SaveData(_englishTitles, StorageFile, null);
                _hasSomethingChanged = false;
            }
        }

        private async Task LoadData()
        {
            _englishTitles = await _cache.RetrieveData<Dictionary<string, string>>(
                StorageFile,
                null,
                0) ?? new Dictionary<string, string>();
        }
    }
}
