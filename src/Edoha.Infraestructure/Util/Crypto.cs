using Edoha.Domain.Interfaces.Infraestructure.Util;
using System.Security.Cryptography;

namespace Edoha.Infraestructure.Util
{
    public class Crypto : ICrypto
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 100000;
        private string UnhashedValue;

        public void SetUnhashedValue(string unhashedValue)
        {
            this.UnhashedValue = unhashedValue;
        }

        public byte[] GetPBKDF2()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] salt = new byte[SaltSize];
                rng.GetBytes(salt); 

                using (var pbkdf2 = new Rfc2898DeriveBytes(UnhashedValue, salt, Iterations, HashAlgorithmName.SHA256))
                {
                    byte[] hash = pbkdf2.GetBytes(HashSize);

                    byte[] hashBytes = new byte[SaltSize + HashSize];
                    Array.Copy(salt, 0, hashBytes, 0, SaltSize);
                    Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

                    return hashBytes;
                }
            }
        }

        public bool ValidatePBKDF2(byte[] hashedValue)
        {
            byte[] pbkdf2 = this.GetPBKDF2();

            if(pbkdf2 == hashedValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
