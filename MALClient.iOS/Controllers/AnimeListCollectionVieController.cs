using Foundation;
using System;
using UIKit;
using MALClient.Models.Enums;
using MALClient.XShared.Utils;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;
using GalaSoft.MvvmLight.Helpers;

namespace MALClient.iOS
{
    public partial class AnimeListCollectionVieController : UIViewController
    {
		AnimeListViewModel VM { get { return ViewModelLocator.AnimeList; } }

		public AnimeListCollectionVieController(IntPtr handle) : base(handle)
		{
		}

		Binding<SmartObservableCollection<AnimeItemViewModel>, SmartObservableCollection<AnimeItemViewModel>> AnimeItemsBinding;

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			Credentials.SetAuthStatus(true);
			Credentials.Update("MALClientTestAcc", "MuchVerificatio", ApiType.Mal);
			ViewModelLocator.AnimeList.Init(null);
			ViewModelLocator.AnimeList.Initialized += AnimeList_Initialized;
			AnimeListCollectionView.RegisterNibForCell(AnimeListCollectionViewCell.Nib, AnimeListCollectionViewCell.Key);
			AnimeItemsBinding = this.SetBinding(() => VM.AnimeItems);
			AnimeItemsBinding.WhenSourceChanges(() => AnimeListCollectionView.AllLoadedAnimeItemAbstractions = VM.AllLoadedAnimeItemAbstractions);
		}

		void AnimeList_Initialized()
		{

		}
	}
}