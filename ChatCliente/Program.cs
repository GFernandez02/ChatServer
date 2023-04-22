using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChatCliente
{
    internal class Program
    {
        static string alias = "usuario nuevo";
        static void Main(string[] args)
        {
            Console.Write("Ingrese su alias: ");
            alias = Console.ReadLine();
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            int port = 5000;
            TcpClient cliente = new TcpClient();
            cliente.Connect(ip, port);
            Console.WriteLine("Se conectó cliente");
            NetworkStream stream = cliente.GetStream();
            Thread t = new Thread((cliente) => RecibirDatos((TcpClient)cliente));
            t.Start(cliente);
            string mensaje;
            while (!string.IsNullOrEmpty((mensaje = Console.ReadLine())))
            {
                mensaje = string.Concat(alias," : " , mensaje);
                byte[] buffer = Encoding.ASCII.GetBytes(mensaje);
                stream.Write(buffer, 0, buffer.Length);

            }
            cliente.Client.Shutdown(SocketShutdown.Send);
            t.Join();
            stream.Close();
            cliente.Close();
            Console.ReadKey();
        }

        private static void RecibirDatos(TcpClient cliente)
        {
            NetworkStream stream = cliente.GetStream();
            byte[] bytesRecibidos = new byte[1024];
            int byteCount;

            while ((byteCount = stream.Read(bytesRecibidos, 0, bytesRecibidos.Length)) > 0)
            {
                Console.Write(Encoding.ASCII.GetString(bytesRecibidos, 0, byteCount));
            }
        }
    }
}