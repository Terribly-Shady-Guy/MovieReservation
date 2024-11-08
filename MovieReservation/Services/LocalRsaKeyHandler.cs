using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace MovieReservation.Services
{
    public class LocalRsaKeyHandler : IRsaKeyHandler
    {
        private readonly string _rsaDirectoryPath = Path.Combine(Environment.CurrentDirectory, "..", "Rsa");

        public void SaveKey()
        {
            var rsa = RSA.Create();
            if (!Directory.Exists(_rsaDirectoryPath))
            {
                Directory.CreateDirectory(_rsaDirectoryPath);
            }

            string privateKey = rsa.ToXmlString(true);
            string publicKey = rsa.ToXmlString(false);

            File.WriteAllText(Path.Combine(_rsaDirectoryPath, "private.xml"), privateKey);
            File.WriteAllText(Path.Combine(_rsaDirectoryPath, "public.xml"), publicKey);
        }

        public bool KeyExists()
        {
            return File.Exists(Path.Combine(_rsaDirectoryPath, "private.xml"))
                && File.Exists(Path.Combine(_rsaDirectoryPath, "public.xml"));
        }

        public async Task<RsaSecurityKey> LoadPublicAsync()
        {
            string publicKey = await File.ReadAllTextAsync(Path.Combine(_rsaDirectoryPath, "public.xml"));

            var rsa = RSA.Create();
            rsa.FromXmlString(publicKey);

            return new RsaSecurityKey(rsa);
        }

        public async Task<RsaSecurityKey> LoadPrivateAsync()
        {
            string privateKey = await File.ReadAllTextAsync(Path.Combine(_rsaDirectoryPath, "private.xml"));

            var rsa = RSA.Create();
            rsa.FromXmlString(privateKey);

            return new RsaSecurityKey(rsa);
        }
    }
}
