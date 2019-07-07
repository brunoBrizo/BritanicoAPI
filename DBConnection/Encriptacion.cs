using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DBConnection
{
    public class Encriptacion
    {
        private static string Patron = "jy7vzQvhjjRWQER";
        internal const string Inputkey = "mbGb8QPUy2Oc9ah8T2Dec6S672F9B6B9hA4";

        public static string Encriptar(string strAEncriptar)
        {
            try
            {
                return EncryptRijndael(strAEncriptar, Patron);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string Desencriptar(string strEncripted)
        {
            try
            {
                return DecryptRijndael(strEncripted, Patron);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string EncryptRijndael(string strAEncriptar, string Patron)
        {
            if (string.IsNullOrEmpty(strAEncriptar))
                throw new Exception("Texto a encriptar no puede ser vacío!");

            var aesAlg = NewRijndaelManaged(Patron);

            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            var msEncrypt = new MemoryStream();
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(strAEncriptar);
            }
            return Convert.ToBase64String(msEncrypt.ToArray());
        }

        private static bool IsBase64String(string base64String)
        {
            base64String = base64String.Trim();
            return (base64String.Length % 4 == 0) &&
                   Regex.IsMatch(base64String, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }

        private static string DecryptRijndael(string cipherText, string Patron)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new Exception("Texto a desencriptar no puede ser vacío!");

            if (!IsBase64String(cipherText))
                throw new Exception("Texto a desencriptar no es Base64 encoded");

            string text;

            var aesAlg = NewRijndaelManaged(Patron);
            var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            var cipher = Convert.FromBase64String(cipherText);

            using (var msDecrypt = new MemoryStream(cipher))
            {
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        text = srDecrypt.ReadToEnd();
                    }
                }
            }
            return text;
        }

        private static RijndaelManaged NewRijndaelManaged(string Patron)
        {
            var saltBytes = Encoding.ASCII.GetBytes(Patron);
            var key = new Rfc2898DeriveBytes(Inputkey, saltBytes);

            var aesAlg = new RijndaelManaged();
            aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
            aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

            return aesAlg;
        }

    }










}
