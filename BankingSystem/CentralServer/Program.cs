using BankingSystem.Models;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace BankingSystem
{
    public class CentralServer
    {
        private static Dictionary<string, User> users = new();
        private static readonly object lockObj = new();

        public static void Main()
        {
            TcpListener server = new(IPAddress.Any, 6000);
            server.Start();
            Console.WriteLine("Centralni server je pokrenut...\n\n");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                new Thread(() => HandleClient(client)).Start();
            }
        }

        private static void HandleClient(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];

                int bytesRead;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string request = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Primljen zahtev: {request}");

                    string response = ProcessRequest(request);

                    Console.WriteLine($"Odgovor: {response}\n");

                    byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                    stream.Write(responseBytes, 0, responseBytes.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška u obradi zahteva: {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }

        private static string ProcessRequest(string request)
        {
            var parts = request.Split('|');
            string command = parts[0].ToUpper();

            Console.WriteLine($"Procesuiram akciju: {command}");

            return command switch
            {
                "REGISTER" => RegisterUser(parts),
                "BALANCE" => GetBalance(parts),
                "TRANSACTION" => ProcessTransaction(parts),
                _ => "Nepostojeca akcija."
            };
        }

        private static string RegisterUser(string[] data)
        {
            if (data.Length != 6) return "Nevalidan format unosa.";

            string username = data[1];
            string firstName = data[3];
            string lastName = data[4];

            if (!System.Text.RegularExpressions.Regex.IsMatch(username, @"^[A-Za-z][A-Za-z0-9]*$"))
            {
                return "Korisnicko ime mora početi slovom i može sadržati samo slova i brojeve.";
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(firstName, @"^[A-Za-z]+$"))
            {
                return "Ime može sadržavati samo slova.";
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(lastName, @"^[A-Za-z]+$"))
            {
                return "Prezime može sadržavati samo slova.";
            }

            firstName = CapitalizeFirstLetter(firstName);
            lastName = CapitalizeFirstLetter(lastName);

            lock (lockObj)
            {
                if (users.ContainsKey(username))
                    return "Korisnik sa ovim korisnickom imenom vec postoji.";

                if (!decimal.TryParse(data[5], out var initialBalance))
                    return "Nevalidan unos za stanje na racunu.";

                var newUser = new User(username, data[2], data[3], data[4], initialBalance);
                users[username] = newUser;

                return "Registracija uspesna.";
            }
        }

        private static string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return char.ToUpper(input[0]) + input.Substring(1).ToLower();
        }

        private static string GetBalance(string[] data)
        {
            if (data.Length != 3) return "Nevalidan format unosa.";

            string username = data[1];
            lock (lockObj)
            {
                if (users.TryGetValue(username, out var user))
                {
                    return user.Password == data[2]
                        ? $"Stanje na racunu: {user.AccountBalance} RSD"
                        : "Pogresna lozinka.";
                }
                return "Korisnik nije pronadjen.";
            }
        }

        private static string ProcessTransaction(string[] data)
        {
            if (data.Length != 4) return "Nevalidan format unosa.";

            string username = data[1];
            lock (lockObj)
            {
                if (!users.TryGetValue(username, out var user))
                    return "Korisnik nije pronadjen.";

                if (user.Password != data[2])
                    return "Pogresna lozinka.";

                if (!decimal.TryParse(data[3], out var amount))
                    return "Nevalidan unos za stanje na racunu.";

                if (user.AccountBalance + amount < 0)
                    return "Nemate dovoljno sredstava na racunu.";

                user.AccountBalance += amount;
                return $"Transakcija uspesna. Stanje na racunu: {user.AccountBalance} RSD";
            }
        }
    }
}
