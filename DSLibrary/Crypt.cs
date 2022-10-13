using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// 문자열 암호화/복호화 클래스
/// </summary>
namespace DSLibrary
{
    class Crypt
    {
        readonly byte[] keyChars = new byte[8];
        readonly string key = string.Empty;

        #region 생성자
        public Crypt(string key)
        {
            this.key = key;
            this.keyChars = ASCIIEncoding.ASCII.GetBytes(this.key);
        }

        #endregion

        #region 메서드
        /// <summary>
        /// 문자열을 암호화한다.
        /// </summary>
        /// <param name="value">암호화 대상 문자열</param>
        /// <returns></returns>
        public string Encrypt(string value)
        {
            try
            {
                if (this.keyChars.Length != 8)
                    throw (new Exception("Invalid key. Key length must be 8 byte."));

                DESCryptoServiceProvider rc2 = new DESCryptoServiceProvider
                {
                    Key = this.keyChars,
                    IV = this.keyChars
                };

                MemoryStream ms = new MemoryStream();
                CryptoStream cryStream = new CryptoStream(ms, rc2.CreateEncryptor(), CryptoStreamMode.Write);
                byte[] data = Encoding.UTF8.GetBytes(value.ToCharArray());

                cryStream.Write(data, 0, data.Length);
                cryStream.FlushFinalBlock();

                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 문자열을 복호화한다.
        /// </summary>
        /// <param name="source">복호화 대상 문자열</param>
        /// <returns></returns>
        public string Decrypt(string value)
        {
            try
            {
                if (value == string.Empty)
                    return string.Empty;

                DESCryptoServiceProvider rc2 = new DESCryptoServiceProvider
                {
                    Key = this.keyChars,
                    IV = this.keyChars
                };

                MemoryStream ms = new MemoryStream();
                CryptoStream cryStream = new CryptoStream(ms, rc2.CreateDecryptor(), CryptoStreamMode.Write);
                byte[] data = Convert.FromBase64String(value);

                cryStream.Write(data, 0, data.Length);
                cryStream.FlushFinalBlock();

                return Encoding.UTF8.GetString(ms.GetBuffer());
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}