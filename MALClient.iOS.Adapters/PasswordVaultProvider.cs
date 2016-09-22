using System;
using Foundation;
using MALClient.Adapters.Credentails;
using MALClient.Models.AdapterModels;
using Security;

namespace MALClient.iOS.Adapters
{
	public class PasswordVaultProvider : IPasswordVault
	{
		static string _generic = "PasswordVault";
		public void Add(VaultCredential credential)
		{
			var rec = new SecRecord(SecKind.GenericPassword)
			{
				Label = credential.Domain,
				Description = credential.Domain,
				Account = credential.UserName,
				Service = credential.Domain,
				ValueData = credential.Password,
				Comment = credential.Domain,
				Generic = NSData.FromString(_generic)
			};
			var sec = SecKeyChain.Add(rec);
			System.Diagnostics.Debug.WriteLine(sec.ToString());
		}

		public VaultCredential Get(string domain)
		{
			var rec = new SecRecord(SecKind.GenericPassword)
			{
				Service = domain,
				Generic = NSData.FromString(_generic)
			};

			SecStatusCode sec;
			var match = SecKeyChain.QueryAsRecord(rec, out sec);

			return new VaultCredential(match.Service, match.Account, match.ValueData.ToString());
		}

		public void Reset()
		{
			var rec = new SecRecord(SecKind.GenericPassword)
			{
				Generic = NSData.FromString(_generic)
			};

			SecStatusCode res;
			var maches = SecKeyChain.QueryAsRecord(rec, int.MaxValue, out res);
			if (res == SecStatusCode.Success)
			{
				foreach (var item in maches)
				{
					SecKeyChain.Remove(item);
				}
			}
		}
	}
}