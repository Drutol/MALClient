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
    [Register ("GeneralAnimeDetailsViewController")]
    partial class GeneralAnimeDetailsViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView GeneralTopTableView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton LoadCharactersButton { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (GeneralTopTableView != null) {
                GeneralTopTableView.Dispose ();
                GeneralTopTableView = null;
            }

            if (LoadCharactersButton != null) {
                LoadCharactersButton.Dispose ();
                LoadCharactersButton = null;
            }
        }
    }
}