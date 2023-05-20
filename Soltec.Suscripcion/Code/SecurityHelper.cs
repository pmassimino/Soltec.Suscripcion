
using System.Text;
using System.Security.Cryptography;


namespace Soltec.Suscripcion.Code
{
    static public class SecurityHelper
    {
        static public string CreatePasswordHash(string pwd, string salt)
        {
            var sha = new SHA1CryptoServiceProvider();
            string saltAndPwd = string.Concat(pwd, salt);
            var UE = new UnicodeEncoding();

            // Convert the string into an array of bytes.
            var MessageBytes = UE.GetBytes(saltAndPwd);
            var hashedPwd = sha.ComputeHash(MessageBytes);
            return Convert.ToBase64String(hashedPwd);
        }
        static public string CreateSalt(int size)
        {
            // Generate a cryptographic random number using the cryptographic
            // service provider
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[size + 1];
            rng.GetBytes(buff);
            // Return a Base64 string representation of the random number
            return Convert.ToBase64String(buff);
        }
    }
}
