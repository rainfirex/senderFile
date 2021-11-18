using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace senderFile.Modules
{
    class Security
    {
        private static byte[] _SALT = new byte[] { 101, 50, 156, 186, 75, 23, 35, 142 };

        private static byte[] ReadByteArray(Stream s)
        {
            byte[] rawLength = new byte[sizeof(int)];
            if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
            {
                throw new SystemException("Stream did not contain properly formatted byte array");
            }

            byte[] buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
            if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                throw new SystemException("Did not read byte array properly");
            }

            return buffer;
        }

        public static string Crypt(string text, string sharedSecret)
        {
            string outText = null;

            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");
            if (string.IsNullOrEmpty(sharedSecret))
                throw new ArgumentNullException("sharedSecret");

            RijndaelManaged aesAlg = null;


            // generate the key from the shared secret and the salt
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _SALT);

            // Create a RijndaelManaged object
            aesAlg = new RijndaelManaged();
            aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for encryption.
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        //Write all data to the stream.
                        swEncrypt.Write(text);
                    }
                }
                outText = Convert.ToBase64String(msEncrypt.ToArray());
            }

            if (aesAlg != null)
                aesAlg.Clear();

            return outText;
        }

        public static string DeCrypt(string text, string sharedSecret)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");
            if (string.IsNullOrEmpty(sharedSecret))
                throw new ArgumentNullException("sharedSecret");

            RijndaelManaged aesAlg = null;

            // Declare the string used to hold
            // the decrypted text.
            string outText = null;

            // generate the key from the shared secret and the salt
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _SALT);

            // Create the streams used for decryption.                
            byte[] bytes = Convert.FromBase64String(text);
            using (MemoryStream msDecrypt = new MemoryStream(bytes))
            {
                // Create a RijndaelManaged object
                // with the specified key and IV.
                aesAlg = new RijndaelManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                // Get the initialization vector from the encrypted stream
                aesAlg.IV = ReadByteArray(msDecrypt);
                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))

                        // Read the decrypted bytes from the decrypting stream
                        // and place them in a string.
                        outText = srDecrypt.ReadToEnd();
                }
            }
            if (aesAlg != null)
                aesAlg.Clear();

            return outText;

        }
    }
}
