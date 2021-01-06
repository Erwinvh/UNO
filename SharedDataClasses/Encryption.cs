using System;
using System.Security.Cryptography;
using System.Text;

namespace SharedDataClasses
{
    public static class Encryption
    {
        private static byte[] key = { 1, 2, 3, 4, 5, 6, 7, 8 };
        private static byte[] iv = { 1, 2, 3, 4, 5, 6, 7, 8 };

        //
        //--Encodes a string text
        //
        public static string Encode(string text)
        {
            SymmetricAlgorithm algorithm = DES.Create();
            ICryptoTransform transform = algorithm.CreateEncryptor(key, iv);
            byte[] inputbuffer = Encoding.Unicode.GetBytes(text);
            byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return Convert.ToBase64String(outputBuffer);
        }

        //
        //--Decodes an encoded string
        //
        public static string Decode(string text)
        {
            SymmetricAlgorithm algorithm = DES.Create();
            ICryptoTransform transform = algorithm.CreateDecryptor(key, iv);
            byte[] inputbuffer = Convert.FromBase64String(text);
            byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return Encoding.Unicode.GetString(outputBuffer);
        }
    }
}

