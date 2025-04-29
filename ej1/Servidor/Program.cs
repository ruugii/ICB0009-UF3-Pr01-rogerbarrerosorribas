using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;
using NetworkStreamNS;
using CarreteraClass;
using VehiculoClass;

namespace Servidor
{

        class Program
    {
        static TcpListener Servidor;
        static string HostName = "localhost";
        static NetworkStream FlujoDatos;

        static void Main(string[] args)
        {            
            //Asignar una IP y Puerto al servidor
            Servidor = new TcpListener(IPAddress.Parse("127.0.0.1"),10002);

            Servidor.Start();
            Console.WriteLine("Servidor: Servidor iniciado");

            while (true)
            {
                TcpClient Cliente = Servidor.AcceptTcpClient();
                
                Thread hilo = new Thread(newVehiculo);
                hilo.Start(Cliente);
            }
        }

        static void newVehiculo(object obj)
        {
            TcpClient Cliente = (TcpClient)obj;
            if (Cliente.Connected)
            {
                try
                {
                    //Transmisión de datos
                Console.WriteLine("Servidor: Cliente conectado");
                FlujoDatos = Cliente.GetStream();

                // NetworkStream 

                byte[] buffer = new byte[1024];
        int bytesLeidos = FlujoDatos.Read(buffer, 0, buffer.Length);

        // Convertir los bytes leídos en texto (suponiendo UTF-8)
        string mensaje = Encoding.UTF8.GetString(buffer, 0, bytesLeidos);

        Console.WriteLine("Servidor: Mensaje recibido del cliente: " + mensaje);

                Vehiculo vehiculo = new Vehiculo();
                Random randId = new Random();
                vehiculo.Id = randId.Next(1,1000);
                vehiculo.Direccion = randId.Next(0,2) == 0 ? "Norte" : "Sur";
                Console.WriteLine("Servidor: Vehiculo creado con id {0} y direccion {1}", vehiculo.Id, vehiculo.Direccion);

                Console.WriteLine(Cliente.GetStream().ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error desconocido: {0}", e.Message);
                }
                
            }
        }
    }
}

