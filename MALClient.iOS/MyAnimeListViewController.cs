using Foundation;
using System;
using UIKit;

namespace MALClient.iOS
{
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
				View.EndEditing(true);
			return true;
		}
	}
}