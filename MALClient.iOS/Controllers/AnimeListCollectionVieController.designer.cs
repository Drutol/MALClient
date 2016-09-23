// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace MALClient.iOS
{
    [Register ("AnimeListCollectionVieController")]
    partial class AnimeListCollectionVieController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MALClient.iOS.AnimeListCollectionView AnimeListCollectionView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (AnimeListCollectionView != null) {
                AnimeListCollectionView.Dispose ();
                AnimeListCollectionView = null;
            }
        }
    }
}