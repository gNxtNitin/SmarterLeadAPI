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
        public string EncryptString(string Message, string Passphrase = null)
        {
            string result = string.Empty;
            if (!string.IsNullOrWhiteSpace(Message))
            {
                byte[] Results;
                System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();

                Passphrase = _configuration.GetValue<string>("encryptionKey");
                MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
                byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));

                // Step 2. Create a new TripleDESCryptoServiceProvider object
                TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

                // Step 3. Setup the encoder
                TDESAlgorithm.Key = TDESKey;
                TDESAlgorithm.Mode = CipherMode.ECB;
                TDESAlgorithm.Padding = PaddingMode.PKCS7;

                // Step 4. Convert the input string to a byte[]
                byte[] DataToEncrypt = UTF8.GetBytes(Message);

                // Step 5. Attempt to encrypt the string
                try
                {
                    ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                    Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
                }
                finally
                {
                    // Clear the TripleDes and Hashprovider services of any sensitive information
                    TDESAlgorithm.Clear();
                    HashProvider.Clear();
                }

                // Step 6. Return the encrypted string as a base64 encoded string
                result = Convert.ToBase64String(Results);

            }
            return result;
        }
        public string DecryptString(string Message, string Passphrase = null)
        {
            string result = string.Empty;
            if (!string.IsNullOrWhiteSpace(Message))
            {
                byte[] Results;
                System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();

                Passphrase = _configuration.GetValue<string>("encryptionKey");
                MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
                byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));

                // Step 2. Create a new TripleDESCryptoServiceProvider object
                TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

                // Step 3. Setup the decoder
                TDESAlgorithm.Key = TDESKey;
                TDESAlgorithm.Mode = CipherMode.ECB;
                TDESAlgorithm.Padding = PaddingMode.PKCS7;

                // Step 4. Convert the input string to a byte[]
                byte[] DataToDecrypt = Convert.FromBase64String(Message);

                // Step 5. Attempt to decrypt the string
                try
                {
                    ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                    Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
                }
                finally
                {
                    // Clear the TripleDes and Hashprovider services of any sensitive information
                    TDESAlgorithm.Clear();
                    HashProvider.Clear();
                }

                // Step 6. Return the decrypted string in UTF8 format
                result = UTF8.GetString(Results);

            }
            return result;
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
