using DidiSoft.Pgp;
using InternetBanking.DataCollections;
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

        #region HMACMD5
        public static string HMD5Hash(string input, string secretKey)
        {
            var key = Encoding.UTF8.GetBytes(secretKey);
            HMACMD5 hmac = new HMACMD5(key);

            // Convert the input string to a byte array and compute the hash.
            byte[] data = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));

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

        public static bool HMD5Verify(string input, string hash, string secretKey)
        {
            // Hash the input.
            string hashOfInput = HMD5Hash(input, secretKey);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            return (0 == comparer.Compare(hashOfInput, hash));
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

    public interface IEncrypt
    {
        public string EncryptData(string msg, string code);
        public bool DecryptData(string signed, string msg = "");
        public void SetKey(string key);
    }

    public class Encrypt : IEncrypt
    {
        // key dùng để phân biệt các ngân hàng 
        private static string _key;
        // RSA: 1, PGP: 2
        private static int _type;
        private string _publicKey;
        private string _privateKey;
        private string _pgpKeyPassword;
        private int _keySize;

        private ILinkingBankCollection _LinkingBankCollection;

        public Encrypt( ILinkingBankCollection linkingBankCollection)
        {
            _LinkingBankCollection = linkingBankCollection;
        }

        private bool Init(int type = 1)
        {
            try
            {
                var linkingBanks = _LinkingBankCollection.Get(new Models.Filters.LinkingBankFilter() { Code = _key }).ToList();
                if (linkingBanks != null)
                {
                    var info = linkingBanks.First(x => (int)x.Type == type);
                    _type = (int)info.Type;
                    _pgpKeyPassword = info.Password;
                    _privateKey = info.PrivateKey;
                    _publicKey = info.PublicKey;
                    _keySize = info.KeySize;
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

        public string EncryptData(string msg, string code ="")
        {
            if (Init())
            {
                if (_type == 1)
                {
                    //SHA256 sha256Hash = SHA256.Create();
                   // byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(msg));
                    //var key = Encoding.UTF8.GetBytes(code);
                    //HMACMD5 hmac = new HMACMD5(key);
                    // Convert the input string to a byte array and compute the hash.
                    //byte[] bytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(msg));

                    var rsa = RSA.Create();
                    rsa.KeySize = _keySize;
                    rsa.ImportRSAPrivateKey(Convert.FromBase64String(_privateKey), out int byteReads);
                    var r = rsa.SignData(Encoding.UTF8.GetBytes(msg), HashAlgorithmName.MD5, RSASignaturePadding.Pkcs1);
                    //string result = Convert.ToBase64String(r);
                    StringBuilder sBuilder = new StringBuilder();

                    // Loop through each byte of the hashed data 
                    // and format each one as a hexadecimal string.
                    for (int i = 0; i < r.Length; i++)
                    {
                        sBuilder.Append(r[i].ToString("x2"));
                    }

                    return sBuilder.ToString();
                }
                else
                {
                    PGPLib pgp = new PGPLib();
                    byte[] bytes = Encoding.UTF8.GetBytes(_privateKey);
                    var stream = new MemoryStream(bytes);
                    var signed = pgp.SignString(msg, stream, _pgpKeyPassword);

                    // Xử lý chuỗi lấy ra base64 string
                    var split = signed.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
                    string result = string.Join("", split.Skip(2).Take(split.Length - 3));
                    //
                    return result;
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
                    //SHA256 sha256Hash = SHA256.Create();
                    //byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(msg));
                    
                    byte[] bsigned = StringToByteArray(signed);
                    byte[] bytes = Encoding.UTF8.GetBytes(msg);
                    var rsa = RSA.Create();
                    rsa.KeySize = _keySize;
                    rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(_publicKey), out int byteReads);
                    var result = rsa.VerifyData(bytes, bsigned, HashAlgorithmName.MD5, RSASignaturePadding.Pkcs1);
                    return result;
                }
                else
                {
                    PGPLib pgp = new PGPLib();
                    byte[] bytes = Encoding.UTF8.GetBytes(_publicKey);
                    var stream = new MemoryStream(bytes);
                    SignatureCheckResult signatureCheck = pgp.VerifyString(signed, stream, out string plainText);
                    var result = signatureCheck == SignatureCheckResult.SignatureVerified;
                    return result;
                }
            }
            else
            {
                return false;
            }
        }

        public void SetKey(string key)
        {
            _key = key;
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
    }
}
