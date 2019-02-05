using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MISTDO.Web.Helpers
{
    public static class GetHash
    {
        public static string SHA512Hash(string secret, string code, string license)
        {
            var key = secret +code + license;
            var data = System.Text.Encoding.UTF8.GetBytes(key);
            using (SHA512 shaM = new SHA512Managed())

            {
                var hash = shaM.ComputeHash(data);
                return GetStringFromHash(hash);
            }


        }
        private static string GetStringFromHash(byte[] hash)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }

            return result.ToString().ToLower();
        }
    }
}
