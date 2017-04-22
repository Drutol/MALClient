using MALClient.Adapters.Credentials;
using MALClient.Models.AdapterModels;

namespace MALClient.WPF.Adapters
{
    public class PasswordVaultProvider : IPasswordVault
    {
        public void Add(VaultCredential credential)
        {

        }

        public VaultCredential Get(string domain)
        {
            return new VaultCredential("","MALClientTestAcc","MuchVerificatio");
        }

        public void Reset()
        {

        }
    }
}