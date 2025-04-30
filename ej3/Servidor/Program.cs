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

        static SemaphoreSlim semaforoTunel = new SemaphoreSlim(1, 1); // solo 1 vehículo a la vez
        static string direccionTunel = ""; // Dirección del vehículo que está usando el túnel
        static readonly object direccionLock = new object();

        static void Main(string[] args)
        {
            // Asignar una IP y Puerto al servidor
            Servidor = new TcpListener(IPAddress.Parse("127.0.0.1"), 10002);
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
                    Console.WriteLine("Servidor: Cliente conectado");
                    NetworkStream FlujoDatos = Cliente.GetStream();
                    string msg = NetworkStreamClass.LeerMensajeNetworkStream(FlujoDatos);
                    Vehiculo vehiculo = new Vehiculo();
                    int nextId = 0;

                    if (msg == "Inicio")
                    {
                        nextId = vehiculos.Count + 1;
                        NetworkStreamClass.EscribirMensajeNetworkStream(FlujoDatos, nextId.ToString());
                        Client clienteAux = new Client(nextId, Cliente);
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

                            bool dentroTunel = false;

                            do
                            {
                                vehiculoAux = NetworkStreamClass.LeerDatosVehiculoNS(FlujoDatos);
                                Console.WriteLine("Servidor: Recibiendo vehiculo con id {0}", vehiculoAux.Id);
                                Console.WriteLine("Servidor: Vehiculo con id {0} en la carretera", vehiculoAux.Id);
                                Console.WriteLine(!vehiculoAux.Acabado);
                                Console.WriteLine(!vehiculoAux.Parado);
                                Console.WriteLine(!vehiculoAux.Acabado && !vehiculoAux.Parado ? "Servidor: Vehiculo en movimiento" : "Servidor: Vehiculo parado");
                                if (!vehiculoAux.Acabado && !vehiculoAux.Parado)
                                {
                                    Console.WriteLine("Servidor: Recibiendo vehiculo con id {0}", vehiculoAux.Id);

                                    // Intentar entrar al túnel solo una vez
                                    if (!dentroTunel)
                                    {
                                        Console.WriteLine("Servidor: Vehiculo {0} esperando acceso al túnel", vehiculoAux.Id);
                                        semaforoTunel.Wait(); // Espera hasta que no haya nadie
                                        Console.WriteLine("Servidor: Vehiculo {0} ENTRA al túnel", vehiculoAux.Id);
                                        dentroTunel = true;

                                        // Permitir que el cliente continúe
                                        vehiculoAux.Parado = false;
                                    }
                                    else
                                    {
                                        // Si ya está dentro del túnel, no se necesita esperar
                                        vehiculoAux.Parado = false;
                                    }
                                    // Actualiza vehículo en la carretera
                                    carretera.ActualizarVehiculo(vehiculoAux);
                                    carretera.MostrarBicicletas();
                                    sendCarretera();
                                }

                            } while (!vehiculoAux.Acabado && !vehiculoAux.Parado);

                            // Cuando termina o se para, libera el túnel
                            if (dentroTunel)
                            {
                                Console.WriteLine("Servidor: Vehiculo {0} SALE del túnel", vehiculoAux.Id);
                                semaforoTunel.Release();
                            }
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

        static void sendCarretera()
        {
            foreach (var client in clientes)
            {
                Console.WriteLine("Servidor: Enviando carretera al cliente {0}", client.Id);
                Console.WriteLine("Servidor: Cliente conectado {0}", client.TcpClient.Connected);
                if (client.TcpClient.Connected)
                {
                    try
                    {
                        NetworkStreamClass.EscribirDatosCarreteraNS(client.Stream, carretera);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error al enviar la carretera: {0}", e.Message);
                    }
                }
            }
        }
    }
}
