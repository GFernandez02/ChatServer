//Se importan los paquetes de datos
using System;
using System.Collections.Specialized;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Text;

//Se inicia el proceso del servidor para abrir puertos y alojar un servidor local
namespace ChatServer
{
    internal class Program
    {
        //Se declaran los objets para poder realizar la conexion con el puerto en un servidor local
        static object _bloqueo=new object();
        static int contador = 1;

        //Se accede al diccionario para poder declarar un TCP/IP el cual, se usara para abrir un puerto local
        static Dictionary<int, TcpClient> clientes=new Dictionary<int, TcpClient>();

        static void Main(string[] args)
        {

            //El TCP utiliza el puerto 5000 para poder realizar cualquier conexión
            TcpListener listener = new TcpListener(IPAddress.Any,5000);
            listener.Start();
            Console.WriteLine("Escuchando puerto 5000");

            //Mientras el TCP de client escuche al TCP de cliente este le asigna una id al alias creado y si es exitoso se muestra un mensaje en pantalla
            while (true)
            {
                TcpClient client= listener.AcceptTcpClient();

                lock(_bloqueo) {
                    clientes.Add(contador, client);
                }
                //Una vez el TCP encuentra conexion con el alias, este aparece en el servidor conectado, con su id respectivamente
                Console.WriteLine("{0} Se conectó", client.Client.RemoteEndPoint.ToString());
                Thread t = new Thread(ManejarClientes); 
                t.Start(contador);
                contador++;
            }
        }

        private static void ManejarClientes(object obj)
        {
            //Se implementa el metodo para poder identificar a cada cuenta con una id y la condición para que pueda enviar un mensaje
            int id = (int)obj;
            TcpClient cliente;
            lock(_bloqueo)
            {
                cliente = clientes[id];


            }
            while (true)
            {
                //Se implementa NetworkStream para poder declarar en bytes la ram a utilizar de este programa
                NetworkStream stream = cliente.GetStream();
                byte[] buffer = new byte[1024];
                int byteCount = stream.Read(buffer, 0, buffer.Length);

                if (byteCount == 0) break;
                //Se abre un string que va a codificar los mensajes por UTF-8 y, el usuario podrá enviar mensajes
                string data = Encoding.UTF8.GetString(buffer, 0, byteCount);


                //whisper id mensaje
                string[] mensaje = data.Split(' ');

                


                if (mensaje[1]== "/whisper")
                {
                    int idTmp= int.Parse(mensaje[2]);
                    // TODO: Que el mensaje real no se junte.
                    string mensajeReal = String.Concat(mensaje[3..]);
                  
                    EnviarMensajePrivado(idTmp, mensajeReal);
                }
                else if (mensaje[1]=="/listall")

                {
                    //TODO: que el mensaje real tambien implemente la id.
                    string clientesTmp = "";
                    foreach (KeyValuePair<int, TcpClient> c in clientes)
                    {
                        clientesTmp=clientesTmp+c.Key+".- "+c.Value.Client.RemoteEndPoint.ToString()+'\n';
                    }
                    EnviarMensajes(clientesTmp);
                }
                else

                {
                    EnviarMensajes(data);
                    Console.WriteLine(data);
                }


            }
            


        }

        private static void EnviarMensajePrivado(int id, string mensaje)

        {
            TcpClient tmp = null;
            clientes.TryGetValue(id, out tmp);

            if (tmp != null)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(mensaje + Environment.NewLine);
                NetworkStream stream = tmp.GetStream();
                stream.Write(buffer, 0, buffer.Length);
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