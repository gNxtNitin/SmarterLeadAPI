using System.Security.Cryptography;

namespace SmarterLead.API.AgencyLeadsManager
{
    public enum ClientEmailResponse
    {

        Email_Response_Interested,
        Email_Response_Not_Interested,
        Email_Response_Already_Insured,
        Email_Response_Need_More_Info
    }
    public class EmailResponseManager
    {
        public static bool ValidateResponse(string responseType, string key, string iv, out string decryptedResponseOut)
        {

            if (string.IsNullOrEmpty(responseType.Trim()))
            {

                decryptedResponseOut = string.Empty;

                return false;
            }



            string dcryptedResponse = DecryptResponse(responseType, key, iv);

            if (Enum.IsDefined(typeof(ClientEmailResponse), dcryptedResponse))
            {
                decryptedResponseOut = dcryptedResponse;
                return true;
            }
            else
            {
                decryptedResponseOut = string.Empty;
                return false;
            }


        }

        private static string DecryptResponse(string encryptedText, string key, string iv)
        {
            string decryptedText = string.Empty;
            encryptedText = encryptedText.Replace('_', '/').Replace('^', '=').Replace('-', '+');

            try
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = System.Text.Encoding.UTF8.GetBytes(key);
                    aesAlg.IV = System.Text.Encoding.UTF8.GetBytes(iv);
                    aesAlg.Mode = CipherMode.CBC;
                    aesAlg.Padding = PaddingMode.PKCS7;


                    using (ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
                    using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedText)))
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        decryptedText = srDecrypt.ReadToEnd();
                    }
                }

            }
            catch (Exception ex)
            {
                decryptedText = string.Empty;
            }

            return decryptedText;
        }

        public static string DecodeEncText(string encryptedText, string salt)
        {
            string decoded = string.Empty;
            string base64Value = encryptedText.Replace('-', '+').Replace('_', '/').Replace('^', '=');

            byte[] decodedBytes = Convert.FromBase64String(base64Value);
            string decodedString = System.Text.Encoding.UTF8.GetString(decodedBytes);

            string reversedSalt = reverseString(salt);
            decoded = decodedString.Replace(salt, "").Replace(reversedSalt, "");

            return reverseString(decoded);

        }

        private static string reverseString(string str)
        {
            char[] c = str.ToCharArray();
            Array.Reverse(c);
            return new String(c);
        }



        private static string EncryptResponse(string plainText, string key, string iv)
        {
            string encryptedText = string.Empty;

            try
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = System.Text.Encoding.UTF8.GetBytes(key);
                    aesAlg.IV = System.Text.Encoding.UTF8.GetBytes(iv);
                    aesAlg.Mode = CipherMode.CBC;
                    aesAlg.Padding = PaddingMode.PKCS7;

                    using (ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }

                        encryptedText = Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                encryptedText = string.Empty;
            }

            return encryptedText;
        }

    }
}
