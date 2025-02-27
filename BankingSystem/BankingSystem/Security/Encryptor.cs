using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Security
{
    public class Encryptor
    {
        public string Encrypt(string data)
        {
            // Implementiraj šifrovanje
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
        }

        public string Decrypt(string encryptedData)
        {
            // Implementiraj dešifrovanje
            return Encoding.UTF8.GetString(Convert.FromBase64String(encryptedData));
        }
    }
}
