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

        static List<Vehiculo> vehiculos = new List<Vehiculo>();

        static List<Client> clientes = new List<Client>();
        static Carretera carretera = new Carretera();
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
                NetworkStream FlujoDatos;
                FlujoDatos = Cliente.GetStream();
                string msg = NetworkStreamClass.LeerMensajeNetworkStream(FlujoDatos);
                Vehiculo vehiculo = new Vehiculo();
                int nextId = 0;
                if (msg == "Inicio")
                {
                    nextId = vehiculos.Count + 1;
                    NetworkStreamClass.EscribirMensajeNetworkStream(FlujoDatos, nextId.ToString());
                    Client clienteAux = new Client(nextId, FlujoDatos);
                    clientes.Add(clienteAux);
                    Console.WriteLine("Servidor: Numero de clientes {0}", clientes.Count);
                    string idRecibido = NetworkStreamClass.LeerMensajeNetworkStream(FlujoDatos);
                    if (int.TryParse(idRecibido, out nextId))
                    {
                        Vehiculo vehiculoAux = NetworkStreamClass.LeerDatosVehiculoNS(FlujoDatos);
                        Console.WriteLine("Servidor: Vehiculo creado con id {0}", vehiculoAux.Id);
                        carretera.AñadirVehiculo(vehiculoAux);
                        carretera.MostrarBicicletas();
                        vehiculos.Add(vehiculo);
                        do
                        {
                            vehiculoAux = NetworkStreamClass.LeerDatosVehiculoNS(FlujoDatos);
                            if (!vehiculoAux.Acabado && !vehiculoAux.Parado)
                            {
                                Console.WriteLine("Servidor: Recibiendo vehiculo con id {0}", vehiculoAux.Id);
                                carretera.ActualizarVehiculo(vehiculoAux);
                                carretera.MostrarBicicletas();
                            }
                        } while (!vehiculoAux.Acabado && !vehiculoAux.Parado);
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

