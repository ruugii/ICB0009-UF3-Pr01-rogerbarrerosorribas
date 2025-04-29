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
                    Console.WriteLine ("Cliente: Cliente conectado");
                    FlujoDatos = Client.GetStream();

                    Console.WriteLine("Cliente: Escribe un mensaje para enviar al servidor:");
                    NetworkStreamClass.EscribirMensajeNetworkStream(FlujoDatos, Console.ReadLine());
                    Console.WriteLine("Cliente: Mensaje enviado.");
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