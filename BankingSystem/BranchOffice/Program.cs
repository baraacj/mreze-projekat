using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace BankingSystem
{
    public class BranchOffice
    {
        private const int udpPort = 11000;
        private const int tcpPort = 12000;
        private UdpClient udpClient;
        private TcpClient tcpClient;
        private NetworkStream stream;

        public void Start()
        {
            // Inicijalizacija UDP komunikacije
            udpClient = new UdpClient();
            string initMessage = "Filijala inicijalizovana";
            byte[] data = Encoding.UTF8.GetBytes(initMessage);
            udpClient.Send(data, data.Length, "localhost", udpPort);

            // Pokretanje TCP klijenta
            tcpClient = new TcpClient("localhost", tcpPort);
            stream = tcpClient.GetStream();
            // Ovdje ide logika za slanje/primanje podataka od servera
        }
    }
}
