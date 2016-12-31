using MALClient.Adapters.Credentails;
using MALClient.Models.AdapterModels;
using MALClient.XShared.ViewModels;

namespace MALClient.Android.Adapters
{
    public class PasswordVaultProvider : IPasswordVault
    {
        public void Add(VaultCredential credential)
        {
            //var vault = new PasswordVault();
            //vault.Add(new PasswordCredential(credential.Domain, credential.UserName, credential.Password));
            ResourceLocator.ApplicationDataService["Username"] = credential.UserName;
            ResourceLocator.ApplicationDataService["Passwd"] = credential.Password;
        }

        public VaultCredential Get(string domain)
        {
            //var vault = new PasswordVault();
            //var credential = vault.FindAllByResource("MALClient").FirstOrDefault();
            //credential.RetrievePassword();
            //return new VaultCredential(credential.Resource, credential.UserName, credential.Password);
            var credential = new VaultCredential("MALClient", ResourceLocator.ApplicationDataService["Username"] as string, ResourceLocator.ApplicationDataService["Passwd"] as string);
            return credential.Password == null || credential.UserName == null ? null : credential;
        }

        public void Reset()
        {
            ResourceLocator.ApplicationDataService["Username"] = null;
            ResourceLocator.ApplicationDataService["Passwd"] = null;
            //var vault = new PasswordVault();
            //foreach (var passwordCredential in vault.RetrieveAll())
            //    vault.Remove(passwordCredential);
        }
    }
}