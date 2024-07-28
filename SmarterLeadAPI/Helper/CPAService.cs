using System.Security.Cryptography;
using System.Text;

namespace SmarterLead.API.Helper
{
    public partial class CPAService
    {
        private IConfiguration _configuration;
        public CPAService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<string> Encrypt(string input, string EncryptionKey = null)
        {
            string result = string.Empty;
            if (!string.IsNullOrWhiteSpace(input))
            {
                string key = _configuration.GetValue<string>("encryptionKey");
                if (!string.IsNullOrWhiteSpace(key))
                {
                    byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input);
                    TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
                    tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
                    tripleDES.Mode = CipherMode.ECB;
                    tripleDES.Padding = PaddingMode.PKCS7;
                    ICryptoTransform cTransform = tripleDES.CreateEncryptor();
                    byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
                    tripleDES.Clear();
                    result = Convert.ToBase64String(resultArray, 0, resultArray.Length);
                }
            }
            return result;
        }

        public async Task<string> Decrypt(string input, string EncryptionKey = null)
        {
            string result = string.Empty;
            if (!string.IsNullOrWhiteSpace(input))
            {
                try
                {
                    string key = _configuration.GetValue<string>("encryptionKey");
                    if (!string.IsNullOrWhiteSpace(key))
                    {
                        byte[] inputArray = Convert.FromBase64String(input);
                        TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
                        tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
                        tripleDES.Mode = CipherMode.ECB;
                        tripleDES.Padding = PaddingMode.PKCS7;
                        ICryptoTransform cTransform = tripleDES.CreateDecryptor();
                        byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
                        tripleDES.Clear();
                        result = UTF8Encoding.UTF8.GetString(resultArray);
                    }
                }
                catch (Exception ex)
                {
                    result = string.Empty;
                }
            }
            return result;
        }
    }
}
