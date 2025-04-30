using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;
using NetworkStreamNS;
using CarreteraClass;
using VehiculoClass;

namespace Client
{
    class Program
    {

        static TcpClient Client;
        static NetworkStream FlujoDatos;

        static void Main(string[] args)
        {
            byte[] bufferLectura = new byte[1024];

            Client = new TcpClient();

            try
            {
                Client.Connect("127.0.0.1",10002);

                if (Client.Connected)
                {
                    Random rand = new Random();
                    Console.WriteLine ("Cliente: Cliente conectado");
                    FlujoDatos = Client.GetStream();
                    NetworkStreamClass.EscribirMensajeNetworkStream(FlujoDatos,"Inicio");
                    string IdRecibido = NetworkStreamClass.LeerMensajeNetworkStream(FlujoDatos);
                    NetworkStreamClass.EscribirMensajeNetworkStream(FlujoDatos, IdRecibido);
                    Vehiculo vehiculo = new Vehiculo();
                    vehiculo.Id = int.Parse(IdRecibido);
                    vehiculo.Pos = 0;
                    vehiculo.Velocidad = rand.Next(100, 2000);
                    vehiculo.Direccion = rand.Next(0,2) == 0 ? "Norte" : "Sur";
                    vehiculo.Acabado = false;
                    vehiculo.Parado = true;
                    NetworkStreamClass.EscribirDatosVehiculoNS(FlujoDatos, vehiculo);
                    Console.WriteLine("Cliente: Vehiculo creado con Id {0}", vehiculo.Id);
                    bool continuar = false;

                    do
                    {
                        NetworkStreamClass.EscribirDatosVehiculoNS(FlujoDatos, vehiculo); // Enviar el estado actual
                        Console.WriteLine("Cliente: Esperando permiso para continuar...");
                        Console.WriteLine("Estado:" );
                        NetworkStreamClass.LeerDatosCarreteraNS(FlujoDatos).MostrarBicicletas();
                        string mensaje = NetworkStreamClass.LeerMensajeNetworkStream(FlujoDatos);
                        Console.WriteLine(mensaje);
                        Console.WriteLine("Cliente: Enviando estado inicial del vehículo y esperando permiso...");

                        continuar = !vehiculo.Parado;

                        if (!continuar)
                        {
                            Console.WriteLine("Cliente: Aún no tengo permiso para continuar, reintentando en 1 segundo...");
                            Thread.Sleep(1000);
                        }
                    } while (!continuar);

                    for (int i = 0; i < 100; i++)
                    {
                        vehiculo.Parado = false;
                        vehiculo.Pos = i + 1;
                        Console.WriteLine("Cliente: Enviando vehiculo con Id {0} y Pos {1}", vehiculo.Id, vehiculo.Pos);
                        NetworkStreamClass.EscribirDatosVehiculoNS(FlujoDatos, vehiculo);
                        Thread.Sleep(vehiculo.Velocidad);
                    }
                    vehiculo.Acabado = true;
                    vehiculo.Parado = true;
                    NetworkStreamClass.EscribirDatosVehiculoNS(FlujoDatos, vehiculo);

                    do
                    {
                        Carretera msg = NetworkStreamClass.LeerDatosCarreteraNS(FlujoDatos);
                        Console.WriteLine("Cliente: Mensaje recibido del servidor:");
                        msg.MostrarBicicletas();
                        Console.WriteLine("-------------------------");
                    } while (true);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine ("Error al conectar con el servidor: {0}", e.Message);
            }
            finally
            {
                FlujoDatos?.Close();
                Client?.Close();
            }
        }
    }
}