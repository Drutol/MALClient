using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Foundation;
using UIKit;

namespace MALClient.iOS
{
	public class AnimeImageDownloaderHelper
	{
		public static Dictionary<string, UIImage> ImageDictionary { get; set; } = new Dictionary<string, UIImage>();

		public static event EventHandler OnImagesLoaded;

		public static Task LoadImages(string[] urls)
		{
			return Task.Run(() =>
			{
				ImageDictionary.Clear();
				foreach (var item in urls)
				{
					var url = NSUrl.FromString(item);
					var data = NSData.FromUrl(url);
					var image = UIImage.LoadFromData(data);
					ImageDictionary.Add(item, image);
				}
				OnImagesLoaded?.Invoke(null,null);
			});
		}
	}
}

