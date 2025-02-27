using System;
using System.Net.Sockets;
using System.Text;

namespace BankingSystem
{
    class Client
    {
        static void Main()
        {
            try
            {
                using TcpClient client = new("localhost", 6000);
                Console.WriteLine("Uspesno povezan na server.\n");
                using NetworkStream stream = client.GetStream();

                Console.WriteLine("REGISTRACIJA NOVOG KORISNIKA");
                Console.WriteLine("-----------------------------------------------------------------");
                Console.WriteLine("REGISTER|username|password|firstName|lastName|initialBalance");
                Console.WriteLine("-----------------------------------------------------------------\n");

                Console.WriteLine("PROVERA STANJA NA RACUNU");
                Console.WriteLine("-----------------------------------------------------------------");
                Console.WriteLine("BALANCE|username|password");
                Console.WriteLine("-----------------------------------------------------------------\n");

                Console.WriteLine("TRANSAKCIJE");
                Console.WriteLine("-----------------------------------------------------------------");
                Console.WriteLine("TRANSACTION|username|password|amount");
                Console.WriteLine("-----------------------------------------------------------------\n");

                while (true)
                {
                    Console.WriteLine("\nUnesite akciju:");
                    string request = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(request))
                    {
                        Console.WriteLine("Unos ne moze biti prazan. Pokusajte ponovo.");
                        continue;
                    }

                    byte[] requestBytes = Encoding.UTF8.GetBytes(request);
                    stream.Write(requestBytes, 0, requestBytes.Length);

                    byte[] responseBytes = new byte[1024];
                    int bytesRead = stream.Read(responseBytes, 0, responseBytes.Length);

                    if (bytesRead == 0)
                    {
                        Console.WriteLine("Veza sa serverom je prekinuta.");
                        break;
                    }

                    string response = Encoding.UTF8.GetString(responseBytes, 0, bytesRead);
                    Console.WriteLine();
                    string tryAgain = "";

                    if (response.Contains("Nepostojeca akcija") || response.Contains("Nevalidan format unosa"))
                    {
                        tryAgain = " Pokusajte ponovo.";
                    }

                    Console.WriteLine($"{response}{tryAgain}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Konekcija sa serverom neuspesna: {ex.Message}");
                Console.ReadLine();
            }
        }
    }
}
