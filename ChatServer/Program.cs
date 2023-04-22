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
                Thread t = new Thread(ManejarClientes); 
                t.Start(contador);
                contador++;
            }
        }

        private static void ManejarClientes(object obj)
        {
            int id = (int)obj;
            TcpClient cliente;
            lock(_bloqueo)
            {
                cliente = clientes[id];


            }
            while (true)
            {
                NetworkStream stream = cliente.GetStream();
                byte[] buffer = new byte[1024];
                int byteCount = stream.Read(buffer, 0, buffer.Length);

                if (byteCount == 0) break;

                string data = Encoding.UTF8.GetString(buffer, 0, byteCount);

                EnviarMensajes(data);
                Console.WriteLine(data);

            }
        }

        private static void EnviarMensajes(string data)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(data +Environment.NewLine);
            lock(_bloqueo)
            {
                foreach (TcpClient c in clientes.Values)
                {
                    NetworkStream stream = c.GetStream();
                    stream.Write(buffer, 0, buffer.Length);
                }
            }
        }
    }
}