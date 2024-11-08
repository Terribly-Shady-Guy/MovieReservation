using Microsoft.IdentityModel.Tokens;

namespace MovieReservation.Services
{
    public interface IRsaKeyHandler
    {
        bool KeyExists();
        Task<RsaSecurityKey> LoadPrivateAsync();
        Task<RsaSecurityKey> LoadPublicAsync();
        void SaveKey();
    }
}