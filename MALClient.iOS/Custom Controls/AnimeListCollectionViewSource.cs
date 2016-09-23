using System;
using System.Collections.Generic;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using UIKit;

namespace MALClient.iOS
{
	public class AnimeListCollectionViewSource : UICollectionViewSource
	{
		List<AnimeItemAbstraction> _allLoadedAnimeItemAbstractions;

		public AnimeListCollectionViewSource(List<AnimeItemAbstraction> allLoadedAnimeItemAbstractions)
		{
			_allLoadedAnimeItemAbstractions = allLoadedAnimeItemAbstractions;
		}

		public override UICollectionViewCell GetCell(UICollectionView collectionView, Foundation.NSIndexPath indexPath)
		{
			var x = collectionView.DequeueReusableCell(AnimeListCollectionViewCell.Key, indexPath) as AnimeListCollectionViewCell;

			x.AnimeItemAbstraction = _allLoadedAnimeItemAbstractions[indexPath.Row];

			return x;
		}

		public override nint GetItemsCount(UICollectionView collectionView, nint section)
		{
			return _allLoadedAnimeItemAbstractions.Count;
		}

	}
}

