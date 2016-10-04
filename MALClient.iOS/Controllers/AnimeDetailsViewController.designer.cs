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
    [Register ("AnimeDetailsViewController")]
    partial class AnimeDetailsViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView AnimeImageView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton MinusButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton MoarButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton PlusButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ScoreLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton StarButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel StatusLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel WatchedEpisodes { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (AnimeImageView != null) {
                AnimeImageView.Dispose ();
                AnimeImageView = null;
            }

            if (MinusButton != null) {
                MinusButton.Dispose ();
                MinusButton = null;
            }

            if (MoarButton != null) {
                MoarButton.Dispose ();
                MoarButton = null;
            }

            if (PlusButton != null) {
                PlusButton.Dispose ();
                PlusButton = null;
            }

            if (ScoreLabel != null) {
                ScoreLabel.Dispose ();
                ScoreLabel = null;
            }

            if (StarButton != null) {
                StarButton.Dispose ();
                StarButton = null;
            }

            if (StatusLabel != null) {
                StatusLabel.Dispose ();
                StatusLabel = null;
            }

            if (WatchedEpisodes != null) {
                WatchedEpisodes.Dispose ();
                WatchedEpisodes = null;
            }
        }
    }
}