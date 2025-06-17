using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace ApplicationLogic.Services
{
    public class LocalRsaKeyHandler : IRsaKeyHandler
    {
        private readonly string _rsaDirectoryPath;
        private readonly string _rsaPrivateKeyPath;
        private readonly string _rsaPublicKeyPath;

        public LocalRsaKeyHandler()
        {
            string rsaDirectoryPath = Path.Combine(Environment.CurrentDirectory, "..", "Rsa");
            rsaDirectoryPath = Path.GetFullPath(rsaDirectoryPath);

            _rsaDirectoryPath = rsaDirectoryPath;
            _rsaPrivateKeyPath = Path.Combine(rsaDirectoryPath, "private.xml");
            _rsaPublicKeyPath = Path.Combine(rsaDirectoryPath, "public.xml");
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
        => File.Exists(_rsaPrivateKeyPath)
            && File.Exists(_rsaPrivateKeyPath);
        

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
