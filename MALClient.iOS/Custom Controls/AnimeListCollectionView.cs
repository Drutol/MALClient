using Foundation;
using System;
using UIKit;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using System.Collections.Generic;

namespace MALClient.iOS
{
    public partial class AnimeListCollectionView : UICollectionView
    {
        public AnimeListCollectionView (IntPtr handle) : base (handle)
        {
        }

		List<AnimeItemAbstraction> _allLoadedAnimeItemAbstractions;
		public List<AnimeItemAbstraction> AllLoadedAnimeItemAbstractions
		{
			get { return _allLoadedAnimeItemAbstractions; }
			set
			{
				Source = new AnimeListCollectionViewSource(value);
				ReloadData();
				_allLoadedAnimeItemAbstractions = value;
			}
		}
    }
}