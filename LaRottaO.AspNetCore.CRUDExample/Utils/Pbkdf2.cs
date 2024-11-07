using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace LaRottaO.AspNetCore.CRUDExample.Utils
{
    public class Pbkdf2
    {
        //Original code by Microsoft
        //Source https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/consumer-apis/password-hashing?view=aspnetcore-2.2

        public static (Boolean success, String errorReason, String hashResult, String saltResult)
            generateHashAndSalt(String argClearTextToHash)
        {
            try
            {
                byte[] salt = new byte[128 / 8];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                }

                String saltResult = Convert.ToBase64String(salt);

                byte[] saltByteArray = Convert.FromBase64String(saltResult);

                string hashedResult = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: argClearTextToHash,
                    salt: saltByteArray,
                    prf: KeyDerivationPrf.HMACSHA512,
                    iterationCount: 100000,
                    numBytesRequested: 512 / 8));

                return (true, "", hashedResult, saltResult);
            }
            catch (Exception ex)
            {
                return (false, ex.ToString(), "", "");
            }
        }

        public (Boolean success, String errorReason, Boolean result) checkIfPasswordIsCorrect(String argStoredHash, String argStoredSalt, String argClearTextToCheck,
            Boolean tryConvertingSaltToBase64First = false)
        {
            try
            {
                /*******************************************************************/
                //   //Some passwords generated with Java's PBKDF2WithHmacSHA512
                //   are not recognized as correct unless the salt string comes
                //   as base64 UTF-8
                /*******************************************************************/

                if (tryConvertingSaltToBase64First)
                {
                    byte[] bytesToEncode = Encoding.UTF8.GetBytes(argStoredSalt);
                    argStoredSalt = Convert.ToBase64String(bytesToEncode);
                }

                byte[] saltByteArray = Convert.FromBase64String(argStoredSalt);

                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: argClearTextToCheck,
                    salt: saltByteArray,
                    prf: KeyDerivationPrf.HMACSHA512,
                    iterationCount: 100000,
                    numBytesRequested: 512 / 8));

                if (!argStoredHash.Equals(hashed) && !tryConvertingSaltToBase64First)
                {
                    return checkIfPasswordIsCorrect(argStoredHash, argStoredSalt, argClearTextToCheck, true);
                }

                return (true, "", argStoredHash.Equals(hashed));
            }
            catch (Exception ex)
            {
                return (false, ex.ToString(), false);
            }
        }
    }
}