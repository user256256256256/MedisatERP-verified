using System.Security.Cryptography;

namespace MedisatERP.Services
{
    // A utility class for encoding and decoding GUIDs and strings using AES encryption
    public class EncryptionService
    {
        // Hardcoded keys for testing
        private static readonly string encryptionKey = "uZBgWRgSYU/Ujusa66dvOLypL534pl7pwhVY1lGUz6I=";
        private static readonly string encryptionIV = "5MCHJ+lM4Q5CCX11a/5IbQ==";

        // Encrypts data using AES
        public static string EncryptString(string value)
        {
            byte[] keyBytes = Convert.FromBase64String(encryptionKey);
            byte[] ivBytes = Convert.FromBase64String(encryptionIV);

            Console.WriteLine($"Key Bytes Length: {keyBytes.Length}"); // Should be 32 for 256 bits
            Console.WriteLine($"IV Bytes Length: {ivBytes.Length}");   // Should be 16 for 128 bits

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = keyBytes;
                aesAlg.IV = ivBytes;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(value);
                        }
                    }

                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        // Decrypts data using AES
        public static string DecryptString(string encoded)
        {
            byte[] keyBytes = Convert.FromBase64String(encryptionKey);
            byte[] ivBytes = Convert.FromBase64String(encryptionIV);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = keyBytes;
                aesAlg.IV = ivBytes;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(encoded)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

        // Encodes a GUID into an encrypted string
        public static string EncryptGuidID(Guid value)
        {
            string guidString = value.ToString();
            return EncryptString(guidString);
        }

        // Decodes an encrypted GUID string back to GUID
        public static Guid DecryptGuidID(string encrypted)
        {
            string decryptedString = DecryptString(encrypted);
            return new Guid(decryptedString);
        }

        // Encodes a string into an encrypted Base64 string
        public static string EncryptBase64String(string value)
        {
            return EncryptString(value);
        }

        // Decodes an encrypted Base64 string back into a regular string
        public static string DecryptBase64String(string encrypted)
        {
            return DecryptString(encrypted);
        }
    }
}