using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketCentroPagos
{
    public class Program
    {
        //permite escuchar y/o enviar datos en protocolos UDP/TCP y otros 
        private static Socket socket = null;
        //nos indicará si el servidor o hilo está escuchando, también nos servirá para finalizarlo 
        private static bool corriendo = false;
        //el punto local es la IP de la tarjeta de RED local por la que escucharemos datos y el puerto 
        private static IPEndPoint puntoLocal = null;

        static void Main(string[] args)
        {
            IPAddress ipEscucha = IPAddress.Parse("127.0.0.1"); //IPAddress.Any; //indicamos que escuche por cualquier tarjeta de red local 
            //IPAddress ipEscucha = IPAddress.Parse("0.0.0.0"); //o podemos indicarle la IP de la tarjeta de red local 
            int puertoEscucha = 8000; //puerto por el cual escucharemos datos             
            puntoLocal = new IPEndPoint(ipEscucha, puertoEscucha); //definimos la instancia del IPEndPoint 
            //lanzamos el escuchador por medio de un hilo 
            new Thread(Escuchador).Start();
            Console.ReadLine(); //esperar a que el usuario escriba algo y de enter 
            corriendo = false; //finalizar el servidor 
        }

        //servidor de escucha de datos UDP, este es llamado por un hilo 
        private static void Escuchador()
        {
            //instanciamos el socket 
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //asociamos el socket a la dirección local por la cual escucharemos (IP:Puerto) 
            //en caso de que otro programa esté escuchado por el mismo IP/Puerto nos lanzará un error aquí 
            socket.Bind(puntoLocal);
            Console.WriteLine("escuchando...");
            //declarar buffer para recibir los datos y le damos un tamaño máximo de datos recibidos por cada mensaje 
            byte[] buffer = new byte[1024];
            //definir objeto para obtener la IP y Puerto de quien nos envía los datos 
            EndPoint ipRemota = new IPEndPoint(IPAddress.Any, 0); //no importa que IPAddress o IP definamos aquí 
            //indicamos que el servidor a partir de aquí está corriendo 
            corriendo = true;
            //ciclo que permitirá escuchar continuamente mientras se esté corriendo el servidor 
            while (corriendo)
            {
                if (socket.Available == 0) //consultamos si hay datos disponibles que no hemos leido 
                {
                    Thread.Sleep(200); //esperamos 200 milisegundos para volver a preguntar 
                    continue; //esta sentencia hace que el programa regrese al ciclo while(corriendo) 
                }
                //en caso de que si hayan datos disponibles debemos leerlos 
                //indicamos el buffer donde se guardarán los datos y enviamos ipRemota como parámetro de referencia 
                //adicionalmente el método ReceiveFrom nos devuelve cuandos bytes se leyeron 
                int contadorLeido = socket.ReceiveFrom(buffer, ref ipRemota);
                //ahora tenemos los datos en buffer (1024 bytes) pero sabemos cuantos recibimos (contadorLeido) 
                //convertimos esos bytes a string 
                string datosRecibidos = Encoding.Default.GetString(buffer, 0, contadorLeido);
                Console.WriteLine("Recibí: " + datosRecibidos);
                Console.ReadLine();
            }
        }
    }
}
