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
                    vehiculo.Velocidad = 0;
                    vehiculo.Direccion = rand.Next(0,2) == 0 ? "Norte" : "Sur";
                    vehiculo.Acabado = false;
                    vehiculo.Parado = true;
                    NetworkStreamClass.EscribirDatosVehiculoNS(FlujoDatos, vehiculo);
                    Console.WriteLine("Cliente: Vehiculo creado con Id {0}", vehiculo.Id);
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