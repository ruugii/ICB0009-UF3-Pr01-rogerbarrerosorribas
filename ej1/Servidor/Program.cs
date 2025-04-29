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

        static List<Vehiculo> vehiculos = new List<Vehiculo>();

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
                string msg = NetworkStreamClass.LeerMensajeNetworkStream(FlujoDatos);
                Vehiculo vehiculo = new Vehiculo();
                int nextId = 0;
                Random randId = new Random();
                if (msg == "Inicio")
                {
                    nextId = vehiculos.Count + 1;
                    NetworkStreamClass.EscribirMensajeNetworkStream(FlujoDatos, nextId.ToString());
                    string idRecibido = NetworkStreamClass.LeerMensajeNetworkStream(FlujoDatos);
                    if (int.TryParse(idRecibido, out nextId))
                    {
                        vehiculo.Id = nextId;
                        vehiculo.Direccion = randId.Next(0,2) == 0 ? "Norte" : "Sur";
                        Console.WriteLine("Servidor: Vehiculo creado con id {0} y direccion {1}", vehiculo.Id, vehiculo.Direccion);
                        Console.WriteLine(Cliente.GetStream().ToString());
                        vehiculos.Add(vehiculo);
                    }
                    else
                    {
                        Console.WriteLine("Servidor: Error al recibir el id del vehiculo");
                        Console.WriteLine("Servidor: El id recibido es {0}", idRecibido);
                        Console.WriteLine("Servidor: El id esperado es {0}", nextId);
                    }
                }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error desconocido: {0}", e.Message);
                }
                
            }
        }
    }
}

