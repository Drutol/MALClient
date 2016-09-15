using System;
using MALClient.Models.Enums;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using UIKit;

namespace MALClient.iOS
{
	public partial class ViewController : UIViewController
	{
		protected ViewController(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.

		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			App.Create();

 			Credentials.SetAuthStatus(true);
			Credentials.Update("MALClientTestAcc", "MuchVerificatio", ApiType.Mal);
			ViewModelLocator.AnimeList.Init(null);
			ViewModelLocator.AnimeList.Initialized += AnimeList_Initialized;
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}

		SmartObservableCollection<AnimeItemViewModel> animeItems;

		void AnimeList_Initialized()
		{
			animeItems = ViewModelLocator.AnimeList.AnimeGridItems;
		}
      
		partial void UIButton17_TouchUpInside(UIButton sender)
		{
			
		}
	}
}