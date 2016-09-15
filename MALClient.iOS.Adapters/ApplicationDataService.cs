using System;
using System.Collections.Generic;
using Foundation;
using MALClient.Adapters;

namespace MALClient.iOS.Adapters
{
	public class ApplicationDataService : IApplicationDataService
	{
		public static readonly NSUserDefaults _userDefaults = NSUserDefaults.StandardUserDefaults;

		public object this[string key]
		{
			get
			{
				return null;
				//throw new NotImplementedException();
			}

			set
			{
				//throw new NotImplementedException();
			}
		}
	}
}
