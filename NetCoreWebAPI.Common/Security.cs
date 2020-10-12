using NetCoreWebAPI.Common;
using NewarePassPort.Common.Enums;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace NewarePassPort.Common
{
    public static class Security
    {
        public static string AesKey()
        {
            string s = string.Empty;
            using (AesManaged am = new AesManaged())
            {
                am.GenerateKey();
                s = Codec.Base64Encode(am.Key);
            }
            return s;
        }
        public static string AesIV()
        {
            string s = string.Empty;
            using (AesManaged am = new AesManaged())
            {
                am.GenerateIV();
                s = Codec.Base64Encode(am.IV);
            }
            return s;
        }
        public static string AesEncrypt(string plainText, string key, string iv)
        {
            if (plainText == null || plainText.Length == 0)
                throw new ArgumentNullException("plainText");
            if (key == null || key.Length == 0)
                throw new ArgumentNullException("key");
            if (iv == null || iv.Length == 0)
                throw new ArgumentNullException("iv");
            string result = string.Empty;
            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Codec.Base64Decode(key);
                    aes.IV = Codec.Base64Decode(iv);
                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter sw = new StreamWriter(cs))
                            {
                                sw.Write(plainText);
                            }
                            result = Codec.Base64Encode(ms.ToArray());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = string.Empty;
            }
            return result;
        }
        public static string AesDecrypt(string cipherText, string key, string iv)
        {
            if (cipherText == null || cipherText.Length == 0)
                throw new ArgumentNullException("cipherText");
            if (key == null || key.Length == 0)
                throw new ArgumentNullException("key");
            if (iv == null || iv.Length == 0)
                throw new ArgumentNullException("iv");
            string result = string.Empty;
            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Codec.Base64Decode(key);
                    aes.IV = Codec.Base64Decode(iv);
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                    using (MemoryStream ms = new MemoryStream(Codec.Base64Decode(cipherText)))
                    {
                        using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader sr = new StreamReader(cs))
                            {
                                result = sr.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch
            {
                result = string.Empty;
            }
            return result;
        }
        public static string Des3Key()
        {
            string s = string.Empty;
            using (TripleDES des = TripleDES.Create())
            {
                s = Codec.Base64Encode(des.Key);
            }
            return s;
        }
        public static string Des3IV()
        {
            string s = string.Empty;
            using (TripleDES des = TripleDES.Create())
            {
                s = Codec.Base64Encode(des.IV);
            }
            return s;
        }
        public static string Des3Encrypt(string plainText, string key, string iv)
        {
            if (plainText == null || plainText.Length == 0)
                throw new ArgumentNullException("plainText");
            if (key == null || key.Length == 0)
                throw new ArgumentNullException("key");
            if (iv == null || iv.Length == 0)
                throw new ArgumentNullException("iv");
            string result = string.Empty;
            try
            {
                using (SymmetricAlgorithm sa = new TripleDESCryptoServiceProvider())
                {
                    sa.Key = Codec.Base64Decode(key);
                    sa.IV = Codec.Base64Decode(iv);
                    sa.Mode = CipherMode.ECB;
                    sa.Padding = PaddingMode.PKCS7;
                    using (ICryptoTransform ct = sa.CreateEncryptor(sa.Key, sa.IV))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            using (CryptoStream cs = new CryptoStream(ms, ct, CryptoStreamMode.Write))
                            {
                                var plainBuffer = Convert.StringToBytes(plainText, Encoding.UTF8);
                                cs.Write(plainBuffer, 0, plainBuffer.Length);
                                cs.FlushFinalBlock();
                                result = Codec.Base64Encode(ms.ToArray());
                            }
                        }
                    }
                }
            }
            catch
            {
                result = "";
            }
            return result;
        }
        public static string Des3Decrypt(string cipherText, string key, string iv)
        {
            if (cipherText == null || cipherText.Length == 0)
                throw new ArgumentNullException("cipherText");
            if (key.Length <= 0)
                throw new ArgumentNullException("key");
            if (iv.Length <= 0)
                throw new ArgumentNullException("iv");
            string result = "";
            try
            {
                using (SymmetricAlgorithm sa = new TripleDESCryptoServiceProvider())
                {
                    sa.Key = Codec.Base64Decode(key);
                    sa.IV = Codec.Base64Decode(iv);
                    sa.Mode = System.Security.Cryptography.CipherMode.ECB;
                    sa.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
                    using (ICryptoTransform ct = sa.CreateDecryptor(sa.Key, sa.IV))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            using (CryptoStream cs = new CryptoStream(ms, ct, CryptoStreamMode.Write))
                            {
                                var cipherBuffer = Codec.Base64Decode(cipherText);
                                cs.Write(cipherBuffer, 0, cipherBuffer.Length);
                                cs.FlushFinalBlock();
                                result = Convert.BytesToString(ms.ToArray(), Encoding.UTF8);
                            }
                        }
                    }
                }
            }
            catch
            {
                result = "";
            }
            return result;
        }
        public static string Sha(string source, ShaType shaType = ShaType.Sha2)
        {
            if (source.Length > 0)
            {
                Encoding encoding = Encoding.UTF8;
                switch (shaType)
                {
                    case ShaType.Sha1:
                        using (SHA1 sha1 = new SHA1CryptoServiceProvider())
                        {
                            return Codec.Base64Encode(
                                sha1.ComputeHash(Convert.StringToBytes(source, encoding)));
                        }
                    case ShaType.Sha2:
                        using (SHA256 sha256 = new SHA256CryptoServiceProvider())
                        {
                            return Codec.Base64Encode(
                                sha256.ComputeHash(Convert.StringToBytes(source, encoding)));
                        }
                    case ShaType.Sha3:
                        using (SHA384 sha384 = new SHA384CryptoServiceProvider())
                        {
                            return Codec.Base64Encode(
                                sha384.ComputeHash(Convert.StringToBytes(source, encoding)));
                        }
                    case ShaType.Sha5:
                        using (SHA512 sha512 = new SHA512CryptoServiceProvider())
                        {
                            return Codec.Base64Encode(
                                sha512.ComputeHash(Convert.StringToBytes(source, encoding)));
                        }
                }
            }
            return "";
        }
        public static string Sha(Stream s, ShaType shaType = ShaType.Sha1)
        {
            switch (shaType)
            {
                default:
                case ShaType.Sha1:
                    using (SHA1 sha = new SHA1Managed())
                    {
                        byte[] hash = sha.ComputeHash(s);
                        return BitConverter.ToString(hash).Replace("-", String.Empty);
                    }
                case ShaType.Sha2:
                    using (SHA256 sha = new SHA256Managed())
                    {
                        byte[] hash = sha.ComputeHash(s);
                        return BitConverter.ToString(hash).Replace("-", String.Empty);
                    }
                case ShaType.Sha3:
                    using (SHA384 sha = new SHA384Managed())
                    {
                        byte[] hash = sha.ComputeHash(s);
                        return BitConverter.ToString(hash).Replace("-", String.Empty);
                    }
                case ShaType.Sha5:
                    using (SHA512 sha = new SHA512Managed())
                    {
                        byte[] hash = sha.ComputeHash(s);
                        return BitConverter.ToString(hash).Replace("-", String.Empty);
                    }
            }
        }
        public static string Md5(string s, Encoding encoding = null)
        {
            if (s == null || s.Length == 0)
                return string.Empty;
            if (encoding == null)
                encoding = Encoding.UTF8;
            using (MD5 md5 = MD5.Create())
            {
                byte[] result = md5.ComputeHash(encoding.GetBytes(s));
                return BitConverter.ToString(result).Replace("-", string.Empty);
            }
        }
        public static string Md5(Stream s)
        {
            if (s.Length == 0)
                return string.Empty;
            using (MD5 md5 = MD5.Create())
            {
                byte[] result = md5.ComputeHash(s);
                return BitConverter.ToString(result).Replace("-", string.Empty);
            }
        }
        public static string Md5(byte[] b)
        {
            if (b == null || b.Length == 0)
                return string.Empty;
            using (MD5 md5 = MD5.Create())
            {
                byte[] result = md5.ComputeHash(b);
                return BitConverter.ToString(result).Replace("-", string.Empty);
            }
        }
        const int RSA_KEY_SIZE = 1024;
        public static bool RsaKey(out string publicKey, out string privateKey)
        {
            try
            {
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(RSA_KEY_SIZE))
                {
                    var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                    var pubKey = rsa.ExportParameters(false);
                    using (StringWriter swPublic = new StringWriter())
                    {
                        xs.Serialize(swPublic, pubKey);
                        publicKey = swPublic.ToString();
                    }
                    var priKey = rsa.ExportParameters(true);
                    using (StringWriter swPrivate = new StringWriter())
                    {
                        xs.Serialize(swPrivate, priKey);
                        privateKey = swPrivate.ToString();
                    }
                }
                return true;
            }
            catch
            {
                publicKey = string.Empty;
                privateKey = string.Empty;
                return false;
            }
        }
        public static string RsaEncrypt(string plainText, string publicKeyString)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(RSA_KEY_SIZE))
            {
                using (StringReader sr = new System.IO.StringReader(publicKeyString))
                {
                    var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                    var publicKey = (RSAParameters)xs.Deserialize(sr);
                    rsa.ImportParameters(publicKey);
                    return System.Convert.ToBase64String(rsa.Encrypt(System.Text.Encoding.Unicode.GetBytes(plainText), true));
                }
            }
        }
        public static string RsaDecrypt(string cipherText, string privateKeyString)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(RSA_KEY_SIZE))
            {
                using (StringReader sr = new System.IO.StringReader(privateKeyString))
                {
                    var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                    var privateKey = (RSAParameters)xs.Deserialize(sr);
                    rsa.ImportParameters(privateKey);
                    return System.Text.Encoding.Unicode.GetString(rsa.Decrypt(System.Convert.FromBase64String(cipherText), true));
                }
            }
        }
        public static string RsaSign(string plainText, string privateKeyString)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(RSA_KEY_SIZE))
            {
                using (StringReader sr = new System.IO.StringReader(privateKeyString))
                {
                    var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                    var privateKey = (RSAParameters)xs.Deserialize(sr);
                    rsa.ImportParameters(privateKey);
                    return System.Convert.ToBase64String(rsa.SignData(System.Text.Encoding.Unicode.GetBytes(plainText), new SHA1CryptoServiceProvider()));
                }
            }
        }
        public static bool RsaSignVerify(string plainText, string signText, string publicKeyString)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(RSA_KEY_SIZE))
            {
                using (StringReader sr = new System.IO.StringReader(publicKeyString))
                {
                    var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                    var publicKey = (RSAParameters)xs.Deserialize(sr);
                    rsa.ImportParameters(publicKey);
                    return rsa.VerifyData(System.Text.Encoding.Unicode.GetBytes(plainText), new SHA1CryptoServiceProvider(), System.Convert.FromBase64String(signText));
                }
            }
        }
        public static string SqlSafe(string sql)
        {
            sql = sql.Replace("'", string.Empty);
            return sql;
        }
    }
}
