using Foundation;
using System;
using UIKit;

namespace MALClient.iOS
{
    public partial class ImageViewController : UIViewController
    {
        public ImageViewController (IntPtr handle) : base (handle)
        {
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var url = NSUrl.FromString("https://myanimelist.cdn-dena.com/images/anime/11/80684.jpg");
			var data = NSData.FromUrl(url);
			var image2 = UIImage.LoadFromData(data);

			image.Image = image2;

		}
    }
}