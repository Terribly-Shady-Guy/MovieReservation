using Microsoft.IdentityModel.Tokens;

namespace ApplicationLogic.Services
{
    public interface IRsaKeyHandler
    {
        bool KeyExists();
        Task<RsaSecurityKey> LoadPrivateAsync();
        Task<RsaSecurityKey> LoadPublicAsync();
        void SaveKey();
    }
}