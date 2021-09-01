//-----------------------------------------------------------------------
// <copyright file="SecurityUtil.cs" company="QJJS">
//     Copyright QJJS. All rights reserved.
// </copyright>
// <author>liyuhang</author>
// <date>2021/9/1 16:26:45</date>
//-----------------------------------------------------------------------

using NETCore.Encrypt;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace FastHttpApi.Utility
{
    public static class SecurityUtil
    {
        private static readonly byte[] IvBytes = { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF };

        #region base64

        public static string Base64Encode(string toBase64)
        {
            var base64EncodedText = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(toBase64));
            return HttpUtility.UrlEncode(base64EncodedText.Replace('+', '-').Replace('/', '_').Replace("=", string.Empty));
        }

        public static string Base64Decode(string base64)
        {
            var decodeBase64 = HttpUtility.UrlDecode(base64);
            if (!string.IsNullOrWhiteSpace(decodeBase64))
            {
                var base64EncodedText = decodeBase64.Replace('-', '+').Replace('_', '/');
                base64EncodedText = base64EncodedText.PadRight(base64EncodedText.Length + ((4 - (base64EncodedText.Length % 4)) % 4), '=');
                return Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedText));
            }

            return string.Empty;
        }

        #endregion base64

        #region 哈希加密算法

        #region MD5 算法

        /// <summary>
        /// MD5 加密
        /// </summary>
        /// <param name="input"> 待加密的字符串 </param>
        /// <param name="encoding"> 字符编码 </param>
        /// <returns>s</returns>
        public static string Md5Encrypt(string input, Encoding encoding)
        {
            return HashEncrypt(MD5.Create(), input, encoding);
        }

        /// <summary>
        /// 验证 MD5 值
        /// </summary>
        /// <param name="input"> 未加密的字符串 </param>
        /// <param name="encoding"> 字符编码 </param>
        /// <returns>s</returns>
        public static bool VerifyMd5Value(string input, Encoding encoding)
        {
            return VerifyHashValue(MD5.Create(), input, Md5Encrypt(input, encoding), encoding);
        }

        /// <summary>
        /// MD5加密密码
        /// </summary>
        /// <param name="code">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>加密后密码</returns>
        public static string Md5Password(string code, string password)
        {
            return Md5Encrypt($"{code}{password}", Encoding.UTF8).ToUpper();
        }

        #endregion MD5 算法

        #region SHA1 算法

        /// <summary>
        /// SHA1 加密
        /// </summary>
        /// <param name="input"> 要加密的字符串 </param>
        /// <param name="encoding"> 字符编码 </param>
        /// <returns>s</returns>
        public static string Sha1Encrypt(string input, Encoding encoding)
        {
            return HashEncrypt(SHA1.Create(), input, encoding);
        }

        /// <summary>
        /// 验证 SHA1 值
        /// </summary>
        /// <param name="input"> 未加密的字符串 </param>
        /// <param name="encoding"> 字符编码 </param>
        /// <returns>s</returns>
        public static bool VerifySha1Value(string input, Encoding encoding)
        {
            return VerifyHashValue(SHA1.Create(), input, Sha1Encrypt(input, encoding), encoding);
        }

        #endregion SHA1 算法

        #region SHA256 算法

        /// <summary>
        /// SHA256 加密
        /// </summary>
        /// <param name="input"> 要加密的字符串 </param>
        /// <param name="encoding"> 字符编码 </param>
        /// <returns>s</returns>
        public static string Sha256Encrypt(string input, Encoding encoding)
        {
            return HashEncrypt(SHA256.Create(), input, encoding);
        }

        /// <summary>
        /// 验证 SHA256 值
        /// </summary>
        /// <param name="input"> 未加密的字符串 </param>
        /// <param name="encoding"> 字符编码 </param>
        /// <returns>s</returns>
        public static bool VerifySha256Value(string input, Encoding encoding)
        {
            return VerifyHashValue(SHA256.Create(), input, Sha256Encrypt(input, encoding), encoding);
        }

        #endregion SHA256 算法

        #region SHA384 算法

        /// <summary>
        /// SHA384 加密
        /// </summary>
        /// <param name="input"> 要加密的字符串 </param>
        /// <param name="encoding"> 字符编码 </param>
        /// <returns>s</returns>
        public static string Sha384Encrypt(string input, Encoding encoding)
        {
            return HashEncrypt(SHA384.Create(), input, encoding);
        }

        /// <summary>
        /// 验证 SHA384 值
        /// </summary>
        /// <param name="input"> 未加密的字符串 </param>
        /// <param name="encoding"> 字符编码 </param>
        /// <returns>s</returns>
        public static bool VerifySha384Value(string input, Encoding encoding)
        {
            return VerifyHashValue(SHA256.Create(), input, Sha384Encrypt(input, encoding), encoding);
        }

        #endregion SHA384 算法

        #region SHA512 算法

        /// <summary>
        /// SHA512 加密
        /// </summary>
        /// <param name="input"> 要加密的字符串 </param>
        /// <param name="encoding"> 字符编码 </param>
        /// <returns>s</returns>
        public static string Sha512Encrypt(string input, Encoding encoding)
        {
            return HashEncrypt(SHA512.Create(), input, encoding);
        }

        /// <summary>
        /// 验证 SHA512 值
        /// </summary>
        /// <param name="input"> 未加密的字符串 </param>
        /// <param name="encoding"> 字符编码 </param>
        /// <returns>s</returns>
        public static bool VerifySha512Value(string input, Encoding encoding)
        {
            return VerifyHashValue(SHA512.Create(), input, Sha512Encrypt(input, encoding), encoding);
        }

        #endregion SHA512 算法

        #region HMAC-MD5 加密

        /// <summary>
        /// HMAC-MD5 加密
        /// </summary>
        /// <param name="input"> 要加密的字符串 </param>
        /// <param name="key"> 密钥 </param>
        /// <param name="encoding"> 字符编码 </param>
        /// <returns>s</returns>
        public static string HmacMd5Encrypt(string input, string key, Encoding encoding)
        {
            return HashEncrypt(new HMACMD5(encoding.GetBytes(key)), input, encoding);
        }

        #endregion HMAC-MD5 加密

        #region HMAC-SHA1 加密

        /// <summary>
        /// HMAC-SHA1 加密
        /// </summary>
        /// <param name="input"> 要加密的字符串 </param>
        /// <param name="key"> 密钥 </param>
        /// <param name="encoding"> 字符编码 </param>
        /// <returns>s</returns>
        public static string HmacSha1Encrypt(string input, string key, Encoding encoding)
        {
            return HashEncrypt(new HMACSHA1(encoding.GetBytes(key)), input, encoding);
        }

        #endregion HMAC-SHA1 加密

        #region HMAC-SHA256 加密

        /// <summary>
        /// HMAC-SHA256 加密
        /// </summary>
        /// <param name="input"> 要加密的字符串 </param>
        /// <param name="key"> 密钥 </param>
        /// <param name="encoding"> 字符编码 </param>
        /// <returns>s</returns>
        public static string HmacSha256Encrypt(string input, string key, Encoding encoding)
        {
            return HashEncrypt(new HMACSHA256(encoding.GetBytes(key)), input, encoding);
        }

        #endregion HMAC-SHA256 加密

        #region HMAC-SHA384 加密

        /// <summary>
        /// HMAC-SHA384 加密
        /// </summary>
        /// <param name="input"> 要加密的字符串 </param>
        /// <param name="key"> 密钥 </param>
        /// <param name="encoding"> 字符编码 </param>
        /// <returns>s</returns>
        public static string HmacSha384Encrypt(string input, string key, Encoding encoding)
        {
            return HashEncrypt(new HMACSHA384(encoding.GetBytes(key)), input, encoding);
        }

        #endregion HMAC-SHA384 加密

        #region HMAC-SHA512 加密

        /// <summary>
        /// HMAC-SHA512 加密
        /// </summary>
        /// <param name="input"> 要加密的字符串 </param>
        /// <param name="key"> 密钥 </param>
        /// <param name="encoding"> 字符编码 </param>
        /// <returns>s</returns>
        public static string HmacSha512Encrypt(string input, string key, Encoding encoding)
        {
            return HashEncrypt(new HMACSHA512(encoding.GetBytes(key)), input, encoding);
        }

        #endregion HMAC-SHA512 加密

        #endregion 哈希加密算法

        #region 对称加密算法

        #region Aes 加解密

        /// <summary>
        ///     Use AES to encrypt strings and process keys as 128 bits
        /// </summary>
        /// <param name="encryptContent">Encrypt content</param>
        /// <param name="secretKey">Secret key</param>
        /// <returns>Base64 string result</returns>
        public static string AesEncrypt(string encryptContent, string secretKey)
        {
            if (string.IsNullOrEmpty(encryptContent))
            {
                return string.Empty;
            }

            try
            {
                byte[] keyArray = Encoding.UTF8.GetBytes(secretKey);

                keyArray = GetAesKey(keyArray);

                byte[] toEncryptArray = Encoding.UTF8.GetBytes(encryptContent);

                SymmetricAlgorithm des = Aes.Create();

                if (des != null)
                {
                    des.Key = keyArray;
                    des.Mode = CipherMode.ECB;
                    des.Padding = PaddingMode.PKCS7;
                    ICryptoTransform cTransform = des.CreateEncryptor();
                    byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                    return Convert.ToBase64String(resultArray);
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return string.Empty;
        }

        /// <summary>
        ///     Use AES to decrypt the string and process the key by 128 bits
        /// </summary>
        /// <param name="decryptContent">Decrypt content</param>
        /// <param name="secretKey">Secret key</param>
        /// <returns>Base64 string result</returns>
        public static string AesDecrypt(string decryptContent, string secretKey)
        {
            try
            {
                byte[] keyArray = Encoding.UTF8.GetBytes(secretKey);

                keyArray = GetAesKey(keyArray);

                byte[] toEncryptArray = Convert.FromBase64String(decryptContent);

                SymmetricAlgorithm des = Aes.Create();

                if (des != null)
                {
                    des.Key = keyArray;
                    des.Mode = CipherMode.ECB;
                    des.Padding = PaddingMode.PKCS7;

                    ICryptoTransform cTransform = des.CreateDecryptor();
                    byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                    return Encoding.UTF8.GetString(resultArray);
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return string.Empty;
        }

        #endregion Aes 加解密

        #endregion 对称加密算法

        #region 非对称加密算法

        /// <summary>
        /// 生成 RSA 公钥和私钥
        /// </summary>
        /// <param name="publicKey"> 公钥 </param>
        /// <param name="privateKey"> 私钥 </param>
        public static void GenerateRsaKeys(out string publicKey, out string privateKey)
        {
            var rsaKey = EncryptProvider.CreateRsaKey();
            publicKey = rsaKey.PublicKey;
            privateKey = rsaKey.PrivateKey;
        }

        /// <summary>
        /// RSA 加密
        /// </summary>
        /// <param name="publicKey"> 公钥 </param>
        /// <param name="content"> 待加密的内容 </param>
        /// <returns> 经过加密的字符串 </returns>
        public static string RsaEncrypt(string publicKey, string content)
        {
            var encrypt = EncryptProvider.RSAEncrypt(publicKey, content, RSAEncryptionPadding.Pkcs1);
            return encrypt;
        }

        /// <summary>
        /// RSA 解密
        /// </summary>
        /// <param name="privateKey"> 私钥 </param>
        /// <param name="content"> 待解密的内容 </param>
        /// <returns> 解密后的字符串 </returns>
        public static string RsaDecrypt(string privateKey, string content)
        {
            var decrypt = EncryptProvider.RSADecrypt(privateKey, content, RSAEncryptionPadding.Pkcs1);
            return decrypt;
        }

        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="privateKey">私钥</param>
        /// <param name="content">需签名的数据</param>
        /// <returns>签名后的值</returns>
        public static string RsaSignature(string privateKey, string content)
        {
            var rsa = EncryptProvider.RSAFromString(privateKey);
            var signature = rsa.SignData(Encoding.UTF8.GetBytes(content), HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
            return Convert.ToBase64String(signature);
        }

        /// <summary>
        /// 签名验证
        /// </summary>
        /// <param name="publicKey">公钥</param>
        /// <param name="content">待验证的数据</param>
        /// <param name="signature">签名</param>
        /// <returns>签名是否符合</returns>
        public static bool RsaVerifySignature(string publicKey, string content, string signature)
        {
            var rsa = EncryptProvider.RSAFromString(publicKey);
            signature = signature.Replace(" ", "+");
            var byteSignature = Convert.FromBase64String(signature);
            var verify = rsa.VerifyData(Encoding.UTF8.GetBytes(content), byteSignature, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
            return verify;
        }

        #endregion 非对称加密算法

        #region 通用加密算法

        /// <summary>
        /// 哈希加密算法
        /// </summary>
        /// <param name="hashAlgorithm"> 所有加密哈希算法实现均必须从中派生的基类 </param>
        /// <param name="input"> 待加密的字符串 </param>
        /// <param name="encoding"> 字符编码 </param>
        /// <returns>s</returns>
        private static string HashEncrypt(HashAlgorithm hashAlgorithm, string input, Encoding encoding)
        {
            var data = hashAlgorithm.ComputeHash(encoding.GetBytes(input));

            return BitConverter.ToString(data).Replace("-", string.Empty);
        }

        /// <summary>
        /// 验证哈希值
        /// </summary>
        /// <param name="hashAlgorithm"> 所有加密哈希算法实现均必须从中派生的基类 </param>
        /// <param name="unhashedText"> 未加密的字符串 </param>
        /// <param name="hashedText"> 经过加密的哈希值 </param>
        /// <param name="encoding"> 字符编码 </param>
        /// <returns>是否验证成功</returns>
        private static bool VerifyHashValue(HashAlgorithm hashAlgorithm, string unhashedText, string hashedText, Encoding encoding)
        {
            return string.Equals(HashEncrypt(hashAlgorithm, unhashedText, encoding), hashedText, StringComparison.OrdinalIgnoreCase);
        }

        #endregion 通用加密算法

        /// <summary>
        ///     128-bit processing key
        /// </summary>
        /// <param name="keyArray">Original byte</param>
        /// <returns>128-bit processing key</returns>
        private static byte[] GetAesKey(byte[] keyArray)
        {
            byte[] newArray = new byte[16];

            if (keyArray.Length >= 16)
            {
                return newArray;
            }

            for (int i = 0; i < newArray.Length; i++)
            {
                if (i >= keyArray.Length)
                {
                    newArray[i] = 0;
                }
                else
                {
                    newArray[i] = keyArray[i];
                }
            }

            return newArray;
        }
    }
}