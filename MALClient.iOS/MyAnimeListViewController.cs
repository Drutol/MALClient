using Foundation;
using System;
using UIKit;
using System.ComponentModel;

namespace MALClient.iOS
{
	//TODO: One base class for MyAnimeListViewController and humhuhmuhmhbirdviewcontroller
	public partial class MyAnimeListViewController : UIViewController
	{

		public MyAnimeListViewController(IntPtr handle) : base(handle)
		{

		}

		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			base.TouchesBegan(touches, evt);
			View.EndEditing(true);
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

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
				PasswordTextField.Layer.BorderColor = new CoreGraphics.CGColor(1, 0, 0);
				PasswordTextField.Layer.BorderWidth = 3;	
			}
			if (UsernameTextField.Text == string.Empty)
			{
				UsernameTextField.Layer.CornerRadius = 5;
				UsernameTextField.Layer.BorderColor = new CoreGraphics.CGColor(1, 0, 0);
				UsernameTextField.Layer.BorderWidth = 3;
			}
			else
			{
				logIn();
			}
		}

		partial void TextField_Editing(UITextField sender)
		{
			sender.Layer.BorderColor = new CoreGraphics.CGColor(0, 0, 0, 0);
		}

		private void logIn()
		{
			//login in logic
		}
	}
}