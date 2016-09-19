using System;
using MALClient.Adapters.Credentails;
using MALClient.Models.AdapterModels;

namespace MALClient.iOS.Adapters
{
	public class PasswordVaultProvider : IPasswordVault
	{
		public void Add(VaultCredential credential)
		{
			//throw new NotImplementedException();
		}

		public VaultCredential Get(string domain)
		{
			return new VaultCredential("MALClient", "MALClientTestAcc", "MuchVerificatio");
		}

		public void Reset()
		{
			//throw new NotImplementedException();
		}
	}
}