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

				List<string> urls = new List<string>();
				foreach (var item in value)
				{
					urls.Add(item.ImgUrl);
				}
				AnimeImageDownloaderHelper.LoadImages(urls.ToArray());
				AnimeImageDownloaderHelper.OnImagesLoaded -= AnimeImageDownloaderHelper_OnImagesLoaded;
				AnimeImageDownloaderHelper.OnImagesLoaded += AnimeImageDownloaderHelper_OnImagesLoaded; 
				_allLoadedAnimeItemAbstractions = value;
			}
		}

		//Problem with loading
		void AnimeImageDownloaderHelper_OnImagesLoaded(object sender, EventArgs e)
		{
			InvokeOnMainThread(() => ReloadData());
		}

	}
}