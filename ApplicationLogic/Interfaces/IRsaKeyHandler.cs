using Microsoft.IdentityModel.Tokens;

namespace ApplicationLogic.Interfaces
{
    public interface IRsaKeyHandler
    {
        bool KeyExists();
        Task<RsaSecurityKey> LoadPrivateAsync();
        Task<RsaSecurityKey> LoadPublicAsync();
        void SaveKey();
    }
}