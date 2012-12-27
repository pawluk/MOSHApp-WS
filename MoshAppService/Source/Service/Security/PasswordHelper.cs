// Project: MoshAppService
// Filename: PasswordHelper.cs
// 
// Author: Jason Recillo

using System;
using System.Security.Cryptography;
using System.Text;

using JetBrains.Annotations;

namespace MoshAppService.Service.Security {
    public static class PasswordHelper {
        /// <summary>
        /// Encrypts a password for storage into the database.
        /// </summary>
        /// <param name="plaintext">The password to encrypt.</param>
        /// <returns>The encrypted password.</returns>
        /// <remarks>The returned password will be exactly 64 characters long.</remarks>
        /// <exception cref="ArgumentNullException">plaintext</exception>
        [NotNull]
        public static string EncryptPassword([NotNull] string plaintext) {
            if (plaintext == null) throw new ArgumentNullException("plaintext");
            var crypto = new SHA256CryptoServiceProvider();
            var data = Encoding.ASCII.GetBytes(plaintext);
            data = crypto.ComputeHash(data);
            return BitConverter.ToString(data).Replace("-", string.Empty);
        }

        /// <summary>
        /// Compares a password with the given encrypted string.
        /// </summary>
        /// <param name="plaintext">The password to check.</param>
        /// <param name="encrypted">The password to compare against.</param>
        /// <returns>True if the passwords match, false otherwise.</returns>
        public static bool CheckPassword(string plaintext, string encrypted) {
            return EncryptPassword(plaintext) == encrypted;
        }
    }
}
