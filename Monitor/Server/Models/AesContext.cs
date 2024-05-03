using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;

namespace MonitorServer.Models
{
    public class AesContext
    {
        private static Dictionary<string, Aes> _aes = new Dictionary<string, Aes>();

        public static void Put(string username, Aes aes)
        {
            _aes[username] = aes;
        }

        public static Aes Get(string username)
        {
            return _aes[username];
        }

        public static async Task<byte[]> Encrypt(byte[] plainBytes, string username)
        {
            byte[] encryptedBytes;

            Aes aes = _aes[username];

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    await cs.WriteAsync(plainBytes);
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        public static byte[] Decrypt(byte[] encryptedBytes, string username)
        {
            byte[] plainBytes;

            Aes aes = _aes[username];

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream msEncrypted = new MemoryStream(encryptedBytes))
            {
                using (CryptoStream cs = new CryptoStream(msEncrypted, decryptor, CryptoStreamMode.Read))
                {
                    using (MemoryStream msPlain = new MemoryStream())
                    {
                        plainBytes = msPlain.ToArray();
                    }
                }
            }

            return plainBytes;
        }

        public static Stream Decrypt(Stream stream, string username)
        {
            Console.WriteLine(username);
            Console.WriteLine(_aes.Keys);
            Aes aes = _aes[username];
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            return new CryptoStream(stream, decryptor, CryptoStreamMode.Read);
        }
    }
}
