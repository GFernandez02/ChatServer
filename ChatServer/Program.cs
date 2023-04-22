using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChatServer
{
    internal class Program
    {
        static object _bloqueo=new object();
        static int contador = 1;

        static Dictionary<int, TcpClient> clientes=new Dictionary<int, TcpClient>();

        static void Main(string[] args)
        {

            TcpListener listener= new TcpListener(IPAddress.Any,5000);
            listener.Start();
            Console.WriteLine("Escuchando puerto 5000");

            while (true)
            {
                TcpClient client= listener.AcceptTcpClient();

                lock(_bloqueo) {
                    clientes.Add(contador, client);
                }
                Console.WriteLine("{0} Se conectó", client.Client.RemoteEndPoint.ToString());
            }
        }
    }
}