using Foundation;
using System;
using System.Collections.Generic;
using UIKit;
using MALClient.XShared.ViewModels;
using MALClient.XShared.ViewModels.Main;
using MALClient.XShared.Utils;
using MALClient.Models.Enums;
using System.Diagnostics;
using GalaSoft.MvvmLight.Helpers;

namespace MALClient.iOS
{
    public partial class AnimeListCollectionViewController : UICollectionViewController
    {
		AnimeListViewModel VM { get { return ViewModelLocator.AnimeList; } }

        public AnimeListCollectionViewController (IntPtr handle) : base (handle)
		{
        }

		Binding<SmartObservableCollection<AnimeItemViewModel>, SmartObservableCollection<AnimeItemViewModel>> animeItemsBinding;

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			Credentials.SetAuthStatus(true);
			Credentials.Update("MALClientTestAcc", "MuchVerificatio", ApiType.Mal);
			VM.Init(null);
			VM.Initialized += VM_Initialized;
			VM.PropertyChanged += VM_PropertyChanged;

			AnimeListCollectionView.RegisterNibForCell(AnimeListCollectionViewCell.Nib, AnimeListCollectionViewCell.Key);

			animeItemsBinding = this.SetBinding(() => VM.AnimeItems);
			animeItemsBinding.WhenSourceChanges(() => AnimeListCollectionView.AllLoadedAnimeItemAbstractions = VM.AllLoadedAnimeItemAbstractions);

		}

		void VM_Initialized()
		{
			VM.LoadAllItemsDetailsCommand.Execute(null);
		}

		void VM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			Debug.WriteLine(e.PropertyName);
			Debug.WriteLine(VM.AllLoadedAnimeItemAbstractions.Count);
			Debug.WriteLine(VM.AnimeItems.Count);
			if (e.PropertyName == "ItemsLoaded")
			{
			}

		}
	}
}