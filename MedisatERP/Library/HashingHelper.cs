using System;

namespace MedisatERP.Library
{
    // A utility class for encoding and decoding GUIDs
    public class HashingHelper
    {
        // Encodes a GUID into a URL-safe Base64 string
        public static string EncodeGuidID(Guid value)
        {
            // Convert the GUID to a byte array
            var bytes = value.ToByteArray();
            // Convert the byte array to a Base64 string and replace URL-unsafe characters
            return Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_'); // URL Safe
        }

        // Decodes a URL-safe Base64 string back into a GUID
        public static Guid DecodeGuidID(string encoded)
        {
            // Replace URL-safe characters back to Base64 original characters
            var base64 = encoded.Replace('-', '+').Replace('_', '/');
            // Convert the Base64 string back to a byte array
            var bytes = Convert.FromBase64String(base64);
            // Convert the byte array back to a GUID
            return new Guid(bytes);
        }
    }
}
