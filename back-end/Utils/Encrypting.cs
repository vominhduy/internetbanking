using DidiSoft.Pgp;
using InternetBanking.Settings;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace InternetBanking.Utils
{
    public class Encrypting
    {
        #region MD5
        private static MD5 md5Hash = MD5.Create();
        public static string MD5Hash(string input)
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        public static bool MD5Verify(string input, string hash)
        {
            // Hash the input.
            string hashOfInput = MD5Hash(input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            return (0 == comparer.Compare(hashOfInput, hash));
        }

        private static byte[] MD5HashBytes(byte[] bytes)
        {
            // Convert the input string to a byte array and compute the hash.
            return md5Hash.ComputeHash(bytes);
        }
        #endregion

        /// <summary>
        /// Encryting input string with Aes crypto
        /// </summary>
        /// <param name="value">Input string</param>
        /// <param name="rgbKey">Secret key for the symmetric algorithm</param>
        /// <param name="rgbIV">Initialization vector (IV) for the symmetric algorithm</param>
        /// <param name="encoding">Input string encoding</param>
        /// <returns>Encrypted base64 string</returns>
        public static string AesEncrypt(string value, byte[] rgbKey, byte[] rgbIV, Encoding encoding)
        {
            using (Aes aes = Aes.Create())
            {
                ICryptoTransform transform = aes.CreateEncryptor(rgbKey, rgbIV);

                using (MemoryStream buffer = new MemoryStream())
                {
                    using (CryptoStream stream = new CryptoStream(buffer, transform, CryptoStreamMode.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(stream, encoding))
                        {
                            writer.Write(value);
                        }
                    }

                    return Convert.ToBase64String(buffer.ToArray());
                }
            }
        }

        /// <summary>
        /// Decryting input string with Aes crypto
        /// </summary>
        /// <param name="text">Input base64 encrypted string</param>
        /// <param name="rgbKey">Secret key for the symmetric algorithm</param>
        /// <param name="rgbIV">Initialization vector (IV) for the symmetric algorithm</param>
        /// <param name="encoding">Input string encoding</param>
        /// <returns>Decrypted string</returns>
        public static string AesDecrypt(string text, byte[] rgbKey, byte[] rgbIV, Encoding encoding)
        {
            using (Aes aes = Aes.Create())
            {
                ICryptoTransform transform = aes.CreateDecryptor(rgbKey, rgbIV);

                using (MemoryStream buffer = new MemoryStream(Convert.FromBase64String(text)))
                {
                    using (CryptoStream stream = new CryptoStream(buffer, transform, CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(stream, encoding))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
        }

        public static string Bcrypt(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool BcryptVerify(string password, string hashPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashPassword);
        }
    }

    public class Encrypt
    {
        private RedisCache _cache = null;

        // key dùng để phân biệt các ngân hàng 
        private readonly string _key;
        // RSA: 1, PGP: 2
        private static int _type;
        private string _publicKey;
        private string _privateKey;
        private string _pgpKeyPassword;

        public Encrypt(string key)
        {
            _key = key;
        }

        private bool Init()
        {
            try
            {
                _cache = new RedisCache("", "");
                if (_cache.StringTryGet(_key, out string valuetype))
                {
                    _type = int.Parse(valuetype.Split('_')[0]);
                    _pgpKeyPassword = valuetype.Split('_')[1];

                    string pattern = _type == 1 ? "rsa" : "pgp";
                    var files = Directory.GetFiles(@"./LocalData", $"{pattern}_{_key}.*").ToList();
                    if (!files.Any())
                    {
                        return false;
                    }
                    else
                    {
                        // RSA
                        if (_type == 1)
                        {
                            _privateKey = File.ReadAllText(files.First(x => x.Contains("private")));
                            _publicKey = File.ReadAllText(files.First(x => x.Contains("public")));
                        }
                        else
                        {
                            _privateKey = files.First(x => x.Contains("private"));
                            _publicKey = files.First(x => x.Contains("public"));
                        }
                    }
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string EncryptData(string msg)
        {
            if (Init())
            {
                if (_type == 1)
                {
                    SHA256 sha256Hash = SHA256.Create();
                    byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(msg));
                    var rsa = RSA.Create();
                    rsa.KeySize = 1024;
                    rsa.ImportRSAPrivateKey(Convert.FromBase64String(_privateKey), out int byteReads);
                    var signed = rsa.SignData(bytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                    string result = Convert.ToBase64String(signed);

                    return result;
                }
                else
                {
                    PGPLib pgp = new PGPLib();
                    var signed = pgp.SignString(msg, new FileInfo(_privateKey), _pgpKeyPassword);
                    return signed;
                }
            }
            else
            {
                return null;
            }
        }

        public bool DecryptData(string signed, string msg = "")
        {
            if (Init())
            {
                if (_type == 1)
                {
                    SHA256 sha256Hash = SHA256.Create();
                    byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(msg));
                    byte[] bsigned = Convert.FromBase64String(signed);
                    var rsa = RSA.Create();
                    rsa.KeySize = 1024;
                    rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(_publicKey), out int byteReads);
                    var result = rsa.VerifyData(bytes, bsigned, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                    return result;
                }
                else
                {
                    PGPLib pgp = new PGPLib();
                    SignatureCheckResult signatureCheck = pgp.VerifyString(signed, new FileInfo(_publicKey), out string plainText);
                    var result = signatureCheck == SignatureCheckResult.SignatureVerified;
                    return result;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
