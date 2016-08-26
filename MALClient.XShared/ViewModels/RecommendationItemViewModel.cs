using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MALClient.Models.Models.AnimeScrapped;
using MALClient.XShared.Delegates;

namespace MALClient.XShared.ViewModels
{
    public class RecommendationItemViewModel
    {
        public RecomendationData Data { get; set; }
        public int Index { get; set; }

        public event EmptyEventHander LoadData;

        public RecommendationItemViewModel(RecomendationData data, int index)
        {
            Data = data;
            Index = index;
        }

        public void PopulateData()
        {
            LoadData?.Invoke();
        }
    }
}
