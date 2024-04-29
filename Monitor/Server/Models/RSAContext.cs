using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace MonitorServer.Models
{
    public class RSAContext
    {
        private static RSA? rsa = null;
        private static void GenerateRSAKey()
        {
            rsa = RSA.Create();
            rsa.KeySize = 1024;
        }

        public static string GetPrivateKeyPem()
        {
            if (rsa == null)
            {
                GenerateRSAKey();
            }

            return rsa!.ExportRSAPrivateKeyPem();
        }

        public static string GetPublicKeyPem()
        {
            if (rsa == null)
            {
                GenerateRSAKey();
            }
            
            return rsa!.ExportRSAPublicKeyPem();
        }

        public static byte[] Encrypt(byte[] data)
        {
            if (rsa == null)
            {
                GenerateRSAKey();
            }

            return rsa!.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
        }

        public static byte[] Decrypt(byte[] data)
        {
            if (rsa == null)
            {
                GenerateRSAKey();
            }

            return rsa!.Decrypt(data, RSAEncryptionPadding.OaepSHA256);
        }
    }
}
