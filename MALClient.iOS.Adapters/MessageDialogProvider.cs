using System;
using MALClient.Adapters;

namespace MALClient.iOS.Adapters
{
	public class MessageDialogProvider : IMessageDialogProvider
	{
		public void ShowMessageDialog(string content, string title)
		{
			throw new NotImplementedException();
		}

		public void ShowMessageDialogWithInput(string content, string title, string trueCommand, string falseCommand, Action callbackOnTrue, Action callBackOnFalse = null)
		{
			throw new NotImplementedException();
		}
	}
}
