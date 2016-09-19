using Foundation;
using System;
using UIKit;
using MALClient.Models.Enums;
using MALClient.iOS.Adapters;
using MALClient.XShared.ViewModels;
using GalaSoft.MvvmLight.Helpers;
using GalaSoft.MvvmLight.Command;
using MALClient.XShared.ViewModels.Main;

namespace MALClient.iOS
{
    public partial class LogInViewController : UIViewController
    {
		LogInViewModel VM { get { return ViewModelLocator.LogIn; } }

        public LogInViewController (IntPtr handle) : base (handle)
        {
        }

		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			base.TouchesBegan(touches, evt);
			View.EndEditing(true);
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			ViewModelLocator.LogIn.Init();
		}

		public Binding<bool, bool> AuthenticatingBinding { get; private set; }
		public Binding<bool, bool> ProblemsButtonBinding { get; private set; }
		public Binding<bool, bool> LogInLogOutBinding { get; private set; }
		public Binding<ApiType, ApiType> CurrentApiTypeBinding { get; private set; }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			App.Create();

			LoginBackgroundView.Layer.CornerRadius = 5;

			AuthenticatingBinding = this.SetBinding(() => VM.Authenticating);
			AuthenticatingBinding.WhenSourceChanges(() => BlurVisualEffect.Hidden = !VM.Authenticating);

			ProblemsButtonBinding = this.SetBinding(() => VM.ProblemsButtonVisibility);
			ProblemsButtonBinding.WhenSourceChanges(() => ProblemsButton.Hidden = !VM.ProblemsButtonVisibility);

			CurrentApiTypeBinding = this.SetBinding(() => VM.CurrentApiType);
			CurrentApiTypeBinding.WhenSourceChanges(() =>
			{
				switch (VM.CurrentApiType)
				{
					case ApiType.Hummingbird:
						HummigbirdButton.BackgroundColor = UIColor.FromRGBA(10, 115, 255, 60);
						MyAnimeListButton.BackgroundColor = UIColor.FromRGBA(10, 115, 255, 30);
						break;
					case ApiType.Mal:
						MyAnimeListButton.BackgroundColor = UIColor.FromRGBA(10, 115, 255, 60);
						HummigbirdButton.BackgroundColor = UIColor.FromRGBA(10, 115, 255, 30);
						break;
				}
			});

			LogInLogOutBinding = this.SetBinding(() => VM.LogOutButtonVisibility);
			LogInLogOutBinding.WhenSourceChanges(() =>
			{
				LogInButton.Hidden = VM.LogOutButtonVisibility;
				LogOutButton.Hidden = !VM.LogOutButtonVisibility;
			});

			HummigbirdButton.SetCommand("TouchUpInside", VM.FocusHumCommand);
			MyAnimeListButton.SetCommand("TouchUpInside", VM.FocusMalCommand);
			ProblemsButton.SetCommand("TouchUpInside", VM.ProblemsCommand);

			UsernameTextField.ShouldReturn += UsernameTextField_ShouldReturn;
			PasswordTextField.ShouldReturn += UsernameTextField_ShouldReturn;
		}

		bool UsernameTextField_ShouldReturn(UITextField textField)
		{
			if (PasswordTextField.Text == string.Empty)
				return PasswordTextField.BecomeFirstResponder();
			else if (UsernameTextField.Text == string.Empty)
				return UsernameTextField.BecomeFirstResponder();
			else
			{
				logIn();
				View.EndEditing(true);
			}
			return true;
		}

		//TODO: Put changing color into custom control with property
		partial void LogInButton_TouchUpInside(UIButton sender)
		{

			if (PasswordTextField.Text == string.Empty)
			{
				PasswordTextField.Layer.CornerRadius = 5;
				PasswordTextField.Layer.BorderColor = UIColor.Red.CGColor;
				PasswordTextField.Layer.BorderWidth = 3;
			}
			if (UsernameTextField.Text == string.Empty)
			{
				UsernameTextField.Layer.CornerRadius = 5;
				UsernameTextField.Layer.BorderColor = UIColor.Red.CGColor;
				UsernameTextField.Layer.BorderWidth = 3;
			}
			else
			{
				logIn();
			}
		}

		partial void TextField_Editing(UITextField sender)
		{
			sender.Layer.BorderColor = UIColor.Clear.CGColor;
		}

		private void logIn()
		{
			VM.PasswordInput = PasswordTextField.Text;
			//VM.UserNameInput = "MALClientTestAcc";
			VM.UserNameInput = UsernameTextField.Text;
			//VM.PasswordInput = "MuchVerificatio";
			VM.LogInCommand.Execute(null);
		}

	}
}