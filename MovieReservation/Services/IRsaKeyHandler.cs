using Microsoft.IdentityModel.Tokens;

namespace DbInfrastructure.Services
{
    public interface IRsaKeyHandler
    {
        bool KeyExists();
        Task<RsaSecurityKey> LoadPrivateAsync();
        Task<RsaSecurityKey> LoadPublicAsync();
        void SaveKey();
    }
}