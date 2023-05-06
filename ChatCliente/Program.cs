//Se implementan paquetes de datos
using System.Net;
using System.Net.Sockets;
using System.Text;

//Se inicia el proceso del programa del cliente para el servidor
namespace ChatCliente
{
    //Se crea la clase interna del programa    
    internal class Program
    {
        //Se crea un metodo para seleccionar un nombre de usuario
        static string alias = "usuario nuevo";
        static void Main(string[] args)
        {
            //Se da la opcion de escribir un alias al gusto del usuario para implementarlo en el servidor
            Console.Write("Ingrese su alias: ");
            alias = Console.ReadLine();
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            //Se abre un puerto determinando donde se alojará este servidor
            int port = 5000;
            TcpClient cliente = new TcpClient();
            //Se realiza la prueba del metodo, para asi lograr una conexion local con el servidor en el puerto 5000
            cliente.Connect(ip, port);
            //Se conecta sin problemas al puerto reservado para abrir servidor
            Console.WriteLine("Se conectó cliente");
            //Con este metodo, se toman los datos del flujo del cliente para que este pueda empezar a enviar un mensaje
            NetworkStream stream = cliente.GetStream();
            Thread t = new Thread((cliente) => RecibirDatos((TcpClient)cliente));
            t.Start(cliente);
            string mensaje;
            //Se da a entender que mientras el cliente envie un mensaje este debe estar con un dato para ser enviado, de lo contrario se termina esta funcion
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
            //Se crea el metodo para poder recibir los datos del cliente hacia el servidor
            NetworkStream stream = cliente.GetStream();
            byte[] bytesRecibidos = new byte[1024];
            int byteCount;
            //Mientras el mensaje a enviar esté en codigo ASCII y su valor sea mayor a 0, este se enviará
            while ((byteCount = stream.Read(bytesRecibidos, 0, bytesRecibidos.Length)) > 0)
            {
                Console.Write(Encoding.ASCII.GetString(bytesRecibidos, 0, byteCount));
            }
        }
    }
}