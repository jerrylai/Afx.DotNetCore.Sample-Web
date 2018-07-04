using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AfxDotNetCoreSample.Common
{
    public static class EncryptUtils
    {
        public static string Encrypt(string input)
        {
            return Afx.Utils.DesUtils.Encrypt(input, ConfigUtils.DesKey,
                System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);
        }

        public static string Decrypt(string input)
        {
            return Afx.Utils.DesUtils.Decrypt(input, ConfigUtils.DesKey,
                System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);
        }

        public static byte[] Encrypt(byte[] buffer)
        {
            return Afx.Utils.DesUtils.Encrypt(buffer, ConfigUtils.DesKey,
                System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);
        }

        public static byte[] Decrypt(byte[] buffer)
        {
            return Afx.Utils.DesUtils.Decrypt(buffer, ConfigUtils.DesKey,
                System.Security.Cryptography.CipherMode.CBC, System.Security.Cryptography.PaddingMode.PKCS7);
        }

        public static string Md5(string input)
        {
            return Afx.Utils.Md5Utils.GetMd5Hash(input);
        }
    }
}
