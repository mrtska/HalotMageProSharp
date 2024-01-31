using System.Security.Cryptography;
using System.Text;

namespace HalotMageProSharp {
    /// <summary>
    /// Crypto utilities for Halot Mage Pro.
    /// </summary>
    public static class CryptoUtil {

        private static readonly byte[] EncryptionKey = [ 0x61, 0x38, 0x35, 0x65, 0x39, 0x64, 0x36, 0x38 ];
        private static readonly byte[] IV = [ 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 ];
        private static readonly byte[] LowerKey = [ 0xAC, 0x47, 0xB0, 0x02, 0x88, 0x15, 0x6E, 0x5C ];

        /// <summary>
        /// Generate a security token for Halot Mage Pro.
        /// </summary>
        /// <param name="password">Raw password.</param>
        /// <returns>Security token</returns>
        public static string GetToken(string password) {

            if (string.IsNullOrEmpty(password)) {
                throw new ArgumentNullException(nameof(password));
            }
            if (password.Length > 8) {
                throw new ArgumentException("Password must be less than or equal to 8 characters.\nPassword can be set more than 8 characters on the printer, but it results in a token error with Halot Box.", nameof(password));
            }

            using var des = DES.Create();
            des.Key = EncryptionKey;

            // Only use the 8 bytes.
            var data = des.EncryptCbc(Encoding.ASCII.GetBytes(password), IV)[0..8];

            // Concat the lower key if the password is more than 4 characters.
            if (password.Length > 4) {
                data = [ .. data, .. LowerKey ];
            }

            return Convert.ToBase64String(data);
        }
    }
}
