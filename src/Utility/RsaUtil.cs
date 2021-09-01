//-----------------------------------------------------------------------
// <copyright file="RsaUtil.cs" company="QJJS">
//     Copyright QJJS. All rights reserved.
// </copyright>
// <author>liyuhang</author>
// <date>2021/9/1 16:20:46</date>
//-----------------------------------------------------------------------
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System;
using System.Security.Cryptography;
using System.Text;

namespace FastHttpApi.Utility
{
    public static class RsaUtil
    {
        /// <summary>
        /// 转换其他平台(java, nodejs)等生成的base64公钥为C# 使用的xml格式
        /// </summary>
        /// <param name="publicKey">base64公钥</param>
        /// <returns>xml公钥</returns>
        public static string GetXmlPublicKeyFormBase64(string publicKey)
        {
            var xmlPublicKey = string.Empty;
            var p = PublicKeyFactory.CreateKey(Convert.FromBase64String(publicKey)) as RsaKeyParameters;

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                RSAParameters rsaParams = new RSAParameters
                {
                    Modulus = p.Modulus.ToByteArrayUnsigned(),
                    Exponent = p.Exponent.ToByteArrayUnsigned()
                };
                rsa.ImportParameters(rsaParams);
                xmlPublicKey = rsa.ToXmlString(false);
            }

            return xmlPublicKey;
        }

        /// <summary>
        /// 转换C# 使用的xml格式公钥为其他平台(java, nodejs)等生成的base64
        /// </summary>
        /// <param name="xmlPublicKey">xml公钥</param>
        /// <returns>base64公钥</returns>
        public static string GetBase64PublicKeyFromXml(string xmlPublicKey)
        {
            var result = string.Empty;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(xmlPublicKey);
                var p = rsa.ExportParameters(false);
                var keyParams = new RsaKeyParameters(false, new BigInteger(1, p.Modulus), new BigInteger(1, p.Exponent));
                var publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(keyParams);
                result = Convert.ToBase64String(publicKeyInfo.ToAsn1Object().GetEncoded());
            }

            return result;
        }

        /// <summary>
        /// 转换其他平台(java, nodejs)等生成的base64私钥为C# 使用的xml格式
        /// </summary>
        /// <param name="privateKey">base64私钥</param>
        /// <returns>xml私钥</returns>
        public static string GetXmlPrivateKeyFormBase64(string privateKey)
        {
            var privateKeyParams = PrivateKeyFactory.CreateKey(Convert.FromBase64String(privateKey)) as RsaPrivateCrtKeyParameters;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                RSAParameters rsaParams = new RSAParameters()
                {
                    Modulus = privateKeyParams.Modulus.ToByteArrayUnsigned(),
                    Exponent = privateKeyParams.PublicExponent.ToByteArrayUnsigned(),
                    D = privateKeyParams.Exponent.ToByteArrayUnsigned(),
                    DP = privateKeyParams.DP.ToByteArrayUnsigned(),
                    DQ = privateKeyParams.DQ.ToByteArrayUnsigned(),
                    P = privateKeyParams.P.ToByteArrayUnsigned(),
                    Q = privateKeyParams.Q.ToByteArrayUnsigned(),
                    InverseQ = privateKeyParams.QInv.ToByteArrayUnsigned()
                };
                rsa.ImportParameters(rsaParams);
                return rsa.ToXmlString(true);
            }
        }

        /// <summary>
        /// 转换C# 使用的xml格式私钥为其他平台(java, nodejs)等生成的base64
        /// </summary>
        /// <param name="xmlPrivateKey">xml私钥</param>
        /// <returns>base64私钥</returns>
        public static string GetBase64PrivateKeyFromXml(string xmlPrivateKey)
        {
            var result = string.Empty;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(xmlPrivateKey);
                var param = rsa.ExportParameters(true);
                var privateKeyParam = new RsaPrivateCrtKeyParameters(new BigInteger(1, param.Modulus), new BigInteger(1, param.Exponent), new BigInteger(1, param.D), new BigInteger(1, param.P), new BigInteger(1, param.Q), new BigInteger(1, param.DP), new BigInteger(1, param.DQ), new BigInteger(1, param.InverseQ));
                var privateKey = PrivateKeyInfoFactory.CreatePrivateKeyInfo(privateKeyParam);

                result = Convert.ToBase64String(privateKey.ToAsn1Object().GetEncoded());
            }

            return result;
        }

        /// <summary>
        /// 使用公钥对字符串
        /// </summary>
        /// <param name="content">要加密的字符串</param>
        /// <param name="publicKey">公钥(base64)</param>
        /// <returns>加密后的内容(base64)</returns>
        public static string EncryptWithPublicKey(string content, string publicKey)
        {
            var encryptedContent = string.Empty;

            using (var rsa = new RSACryptoServiceProvider())
            {
                var xmlPublicKey = GetXmlPublicKeyFormBase64(publicKey);
                rsa.FromXmlString(xmlPublicKey);
                byte[] encryptedData = rsa.Encrypt(Encoding.Default.GetBytes(content), false);
                encryptedContent = Convert.ToBase64String(encryptedData);
            }

            return encryptedContent;
        }

        /// <summary>
        /// 使用私钥解密
        /// </summary>
        /// <param name="content">要解密的字符串</param>
        /// <param name="privateKey">私钥钥(base64)</param>
        /// <returns>解密的内容(base64)</returns>
        public static string DecryptWithPrivateKey(string content, string privateKey)
        {
            string decryptedContent = string.Empty;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                var xmlPrivateKey = GetXmlPrivateKeyFormBase64(privateKey);
                rsa.FromXmlString(xmlPrivateKey);
                byte[] decryptedData = rsa.Decrypt(Convert.FromBase64String(content), false);
                decryptedContent = Convert.ToBase64String(decryptedData);
            }

            return decryptedContent;
        }
    }
}