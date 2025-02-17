using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Jarvis.Shared.Utility
{
    public static class PasswordHashUtil
    {
        public static string CreateHashPassword(string password)
        {
            try
            {
                byte[] bytes = Encoding.Unicode.GetBytes(password);
                byte[] inArray = HashAlgorithm.Create("SHA1").ComputeHash(bytes);
                string hashpassword = Convert.ToBase64String(inArray);
                return hashpassword;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}
