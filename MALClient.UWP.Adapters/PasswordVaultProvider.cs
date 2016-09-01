using System.Linq;
using Windows.Security.Credentials;
using MALClient.Adapters.Credentails;
using MALClient.Models.AdapterModels;

namespace MALClient.UWP.Adapters
{
    public class PasswordVaultProvider : IPasswordVault
    {
        public void Add(VaultCredential credential)
        {
            var vault = new PasswordVault();
            vault.Add(new PasswordCredential(credential.Domain, credential.UserName, credential.Password));
        }

        public VaultCredential Get(string domain)
        {
            var vault = new PasswordVault();
            var credential = vault.FindAllByResource("MALClient").FirstOrDefault();
            credential.RetrievePassword();
            return new VaultCredential(credential.Resource, credential.UserName, credential.Password);
        }

        public void Reset()
        {
            var vault = new PasswordVault();
            foreach (var passwordCredential in vault.RetrieveAll())
                vault.Remove(passwordCredential);
        }
    }
}