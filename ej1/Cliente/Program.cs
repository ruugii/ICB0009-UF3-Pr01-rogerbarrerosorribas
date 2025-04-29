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
                Client.Connect("127.0.0.1",10001);

                if (Client.Connected)
                {
                    Console.WriteLine ("Cliente: Cliente conectado");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine ("Error al conectar con el servidor: {0}", e.Message);
            }
            // try
            // {
            //     Client.Connect("127.0.0.1",10001);
            //     if (Client.Connected)
            //     {
            //         Console.WriteLine ("Cliente: Cliente conectado");

            //         FlujoDatos = Client.GetStream();
            //         //Aquí añadiríamos el Handshake

            //         //Transmisión de datos
            //         string mensaje ="";
            //         do
            //         {
            //             if (FlujoDatos.CanWrite)
            //             {
            //                 mensaje = Console.ReadLine();
            //                 m.Text = mensaje;

            //                 EscribirMensajeNetworkStream(m);

            //                 FlujoDatos.Write(MensajeBytes,0,MensajeBytes.Length);*/
            //                 m = LeerMensajeNetWorkStream();
            //                 Console.WriteLine (m.Text);
            //             }
            //         }while (mensaje !="quit");
            //     }
            // }
            // catch (Exception e)
            // {
            //     Console.WriteLine ("Error al conectar con el servidor: {0}", e.Message);
            // }

            Console.ReadLine();
        }

    }
}