using System;
using UIKit;

namespace MALClient.iOS
{
	public class AnimeListCollectionViewDelegateFlowLayout : UICollectionViewDelegateFlowLayout
	{
		CoreGraphics.CGSize _windowSize = new CoreGraphics.CGSize(100, 100);
		double _pictureWidth = 320;
		double _imageRatio = 1;
		public int ItemsInRowCount { get; set; } = 2;

		public AnimeListCollectionViewDelegateFlowLayout(CoreGraphics.CGSize windowSize, double imageRatio)
		{
			_windowSize = windowSize;
			_imageRatio = imageRatio;
		}

		public override CoreGraphics.CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, Foundation.NSIndexPath indexPath)
		{
			double itemsInRowCount;
			double width;
			if (UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.Portrait)
			{
				itemsInRowCount = ItemsInRowCount;
				width = _windowSize.Height < _windowSize.Width? 
				                   (_windowSize.Height / itemsInRowCount) - (itemsInRowCount - 1) * 1
				                   : 
				                   (_windowSize.Width / itemsInRowCount) - (itemsInRowCount - 1) * 1;
			}
			else
			{
				itemsInRowCount = ItemsInRowCount + 1;
				width = _windowSize.Width < _windowSize.Height ?
				   (_windowSize.Height / itemsInRowCount) - (itemsInRowCount - 1) * 1
				   :
				   (_windowSize.Width / itemsInRowCount) - (itemsInRowCount - 1) * 1;
			}

			double height = width * _imageRatio;
			return new CoreGraphics.CGSize(width, height);
		}
	}
}

