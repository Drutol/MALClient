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
    [Register ("LogInViewController")]
    partial class LogInViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIVisualEffectView Bluuuur { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton HummigbirdButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton LogInButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton MyAnimeListButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField PasswordTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ProblemsButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton RegisterButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField UsernameTextField { get; set; }

        [Action ("HummigbirdButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void HummigbirdButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("LogInButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void LogInButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("MyAnimeListButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void MyAnimeListButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("TextField_Editing:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void TextField_Editing (UIKit.UITextField sender);

        void ReleaseDesignerOutlets ()
        {
            if (Bluuuur != null) {
                Bluuuur.Dispose ();
                Bluuuur = null;
            }

            if (HummigbirdButton != null) {
                HummigbirdButton.Dispose ();
                HummigbirdButton = null;
            }

            if (LogInButton != null) {
                LogInButton.Dispose ();
                LogInButton = null;
            }

            if (MyAnimeListButton != null) {
                MyAnimeListButton.Dispose ();
                MyAnimeListButton = null;
            }

            if (PasswordTextField != null) {
                PasswordTextField.Dispose ();
                PasswordTextField = null;
            }

            if (ProblemsButton != null) {
                ProblemsButton.Dispose ();
                ProblemsButton = null;
            }

            if (RegisterButton != null) {
                RegisterButton.Dispose ();
                RegisterButton = null;
            }

            if (UsernameTextField != null) {
                UsernameTextField.Dispose ();
                UsernameTextField = null;
            }
        }
    }
}