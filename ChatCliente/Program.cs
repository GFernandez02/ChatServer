using System.Net;
using System.Net.Sockets;

namespace ChatCliente
{
    internal class Program
    {
        static string alias = "usuario nuevo";
        static void Main(string[] args)
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            int port = 5000;
            TcpClient cliente = new TcpClient();
            cliente.Connect(ip, port);
            Console.WriteLine("Se conectó cliente");
            string s = Console.ReadLine();
        }
    }
}