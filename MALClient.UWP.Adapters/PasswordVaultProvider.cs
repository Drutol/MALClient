using System.Linq;
using Windows.Security.Credentials;
using MALClient.Adapters.Credentials;
using MALClient.Models.AdapterModels;

namespace MALClient.UWP.Adapters
{
    public class PasswordVaultProvider : IPasswordVault
    {
        private const string NullUserName = "<><>!Null!<><>";

        public void Add(VaultCredential credential)
        {
            var vault = new PasswordVault();
            vault.Add(new PasswordCredential(credential.Domain, string.IsNullOrEmpty(credential.UserName) ? NullUserName : credential.UserName, credential.Password));
        }

        public VaultCredential Get(string domain)
        {
            var vault = new PasswordVault();
            var credential = vault.FindAllByResource(domain).FirstOrDefault();
            credential.RetrievePassword();
            return new VaultCredential(credential.Resource, credential.UserName == NullUserName ? "" : credential.UserName, credential.Password);
        }

        public void Reset()
        {
            var vault = new PasswordVault();
            foreach (var passwordCredential in vault.RetrieveAll())
                vault.Remove(passwordCredential);
        }
    }
}