using System;
using System.Threading.Tasks;
using Foundation;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using UIKit;

namespace MALClient.iOS
{
	public partial class AnimeListCollectionViewCell : UICollectionViewCell
	{
		public static readonly NSString Key = new NSString("AnimeListCollectionViewCell");
		public static readonly UINib Nib;

		public AnimeItemAbstraction AnimeItemAbstraction { get; set; }

		static AnimeListCollectionViewCell()
		{
			Nib = UINib.FromName("AnimeListCollectionViewCell", NSBundle.MainBundle);
		}

		public static AnimeListCollectionViewCell Create()
		{
			return (AnimeListCollectionViewCell)Nib.Instantiate(null, null)[0];
		}

		protected AnimeListCollectionViewCell(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override async void LayoutSubviews()
		{
			base.LayoutSubviews();

			TitleLabel.Text = AnimeItemAbstraction.ViewModel.Title;

			await LoadImage(AnimeItemAbstraction.ViewModel.ImgUrl);
		}

		public Task LoadImage(string imageUrl)
		{
			return Task.Run(() =>
			{
				var url = NSUrl.FromString(imageUrl);
				var data = NSData.FromUrl(url);
				var image = UIImage.LoadFromData(data);

				InvokeOnMainThread(() => AnimeCoverImageView.Image = image);
			});
		}
	}
}
