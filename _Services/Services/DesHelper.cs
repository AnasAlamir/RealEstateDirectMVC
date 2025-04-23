
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace _Services.Services
{
    public static class DesHelper
    {
        // 8-byte key (DES requires 64-bit key)
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("abcdefgh"); // Replace with your secret key

        // 8-byte IV (Initialization Vector)
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("12345678"); // Replace with your IV

        public static string Encrypt(string plainText)
        {
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(Key, IV), CryptoStreamMode.Write))
                {
                    byte[] data = Encoding.UTF8.GetBytes(plainText);
                    cs.Write(data, 0, data.Length);
                    cs.FlushFinalBlock();
                }
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public static string Decrypt(string encryptedText)
        {
            byte[] data = Convert.FromBase64String(encryptedText);
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(Key, IV), CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);
                    cs.FlushFinalBlock();
                }
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }
}




