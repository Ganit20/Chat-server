using System;
using System.IO;
using System.Security.Cryptography;

namespace App3
{
    class Encryption
    {
        public byte[] Encrypt(string original)
        {
            
            using (Aes myAes = Aes.Create())
            {
                
                byte[] encrypted = EncryptStringToBytes_Aes(original);
                return encrypted;
            }
        }
        public string Decrypt(byte[] message)
        {
            using (Aes myAes = Aes.Create())
            {

                string decrypted = DecryptStringFromBytes_Aes(message);
                return decrypted;

            }
        }



        static byte[] EncryptStringToBytes_Aes(string Text)
        {
            var parkey = "38164530425560227810402161222997";
            var Vector = "7088183594888843";
            using (var aesManag = new AesManaged())
            {
                aesManag.KeySize = 256;
                
            }
                if (Text == null || Text.Length <= 0)
                    throw new ArgumentNullException("plainText");
            if (parkey == null || parkey.Length <= 0)
                throw new ArgumentNullException("Key");
            if (Vector == null || Vector.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            using (Aes aesAlg = Aes.Create())
            {

                aesAlg.Key = System.Text.Encoding.UTF8.GetBytes(parkey);
                aesAlg.IV = System.Text.Encoding.UTF8.GetBytes(Vector);
                
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);


                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(Text);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return encrypted;

        }

        static string DecryptStringFromBytes_Aes(byte[] cipherText)
        {
            var parkey = "38164530425560227810402161222997";
            var Vector = "7088183594888843";
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (parkey == null || parkey.Length <= 0)
                throw new ArgumentNullException("Key");
            if (Vector == null || Vector.Length <= 0)
                throw new ArgumentNullException("IV");

            string plaintext = null;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = System.Text.Encoding.UTF8.GetBytes(parkey);
                aesAlg.IV = System.Text.Encoding.UTF8.GetBytes(Vector);

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;

        }
    }
}