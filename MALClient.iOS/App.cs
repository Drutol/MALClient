using System;
using MALClient.iOS.ViewModel;
using MALClient.XShared.ViewModels;

namespace MALClient.iOS
{
	public static class App
	{
		private static bool isCreated = false;

		public static void Create()
		{
			if (isCreated) return;
			iOSViewModelLocator.RegisterDependencies();
			new ViewModelLocator();
			isCreated = !isCreated;
		}
	}
}

