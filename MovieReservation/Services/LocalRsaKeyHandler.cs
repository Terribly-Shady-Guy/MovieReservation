using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace MovieReservation.Services
{
    public class LocalRsaKeyHandler : IRsaKeyHandler
    {
        private readonly string _rsaDirectoryPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "..", "Rsa"));
        private readonly string _rsaPrivateKeyPath;
        private readonly string _rsaPublicKeyPath;

        public LocalRsaKeyHandler()
        {
            _rsaPrivateKeyPath = Path.Combine(_rsaDirectoryPath, "private.xml");
            _rsaPublicKeyPath = Path.Combine(_rsaDirectoryPath, "public.xml");
        }

        public void SaveKey()
        {
            var rsa = RSA.Create();
            if (!Directory.Exists(_rsaDirectoryPath))
            {
                Directory.CreateDirectory(_rsaDirectoryPath);
            }

            string privateKey = rsa.ToXmlString(true);
            string publicKey = rsa.ToXmlString(false);

            File.WriteAllText(_rsaPrivateKeyPath, privateKey);
            File.WriteAllText(_rsaPublicKeyPath, publicKey);
        }

        public bool KeyExists()
        {
            return File.Exists(_rsaPrivateKeyPath)
                && File.Exists(_rsaPrivateKeyPath);
        }

        public async Task<RsaSecurityKey> LoadPublicAsync()
        {
            string publicKey = await File.ReadAllTextAsync(_rsaPublicKeyPath);

            var rsa = RSA.Create();
            rsa.FromXmlString(publicKey);

            return new RsaSecurityKey(rsa);
        }

        public async Task<RsaSecurityKey> LoadPrivateAsync()
        {
            string privateKey = await File.ReadAllTextAsync(_rsaPrivateKeyPath);

            var rsa = RSA.Create();
            rsa.FromXmlString(privateKey);

            return new RsaSecurityKey(rsa);
        }
    }
}
