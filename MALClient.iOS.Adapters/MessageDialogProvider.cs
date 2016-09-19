using System;
using MALClient.Adapters;
using UIKit;

namespace MALClient.iOS.Adapters
{
	public class MessageDialogProvider : IMessageDialogProvider
	{
		public void ShowMessageDialog(string content, string title)
		{
			UIAlertView alertView = new UIAlertView() { Message = content, Title = title };
			alertView.AddButton("Ok");
			alertView.Show();
		}

		public void ShowMessageDialogWithInput(string content, string title, string trueCommand, string falseCommand, Action callbackOnTrue, Action callBackOnFalse = null)
		{
			throw new NotImplementedException();
		}
	}
}
