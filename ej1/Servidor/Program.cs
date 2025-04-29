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
            Servidor = new TcpListener(IPAddress.Parse("127.0.0.1"),10001);

            Servidor.Start();
            Console.WriteLine("Servidor: Servidor iniciado");

            TcpClient Cliente = Servidor.AcceptTcpClient();            

            if (Cliente.Connected)
            {    
                Console.WriteLine("Servidor: Cliente conectado");

                FlujoDatos = Cliente.GetStream();
                //En este punto realizaríamos el handshake

                
            }
        }
    }
}

