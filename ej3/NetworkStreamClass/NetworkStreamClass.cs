using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using VehiculoClass;
using CarreteraClass;

namespace NetworkStreamNS
{
    public class NetworkStreamClass
    {
        // Método para escribir en un NetworkStream los datos de tipo Carretera
        public static void EscribirDatosCarreteraNS(NetworkStream NS, Carretera C)
        {
            byte[] datos = C.CarreteraABytes();
            byte[] longitud = BitConverter.GetBytes(datos.Length); // 4 bytes

            NS.Write(longitud, 0, longitud.Length); // primero escribes la longitud
            NS.Write(datos, 0, datos.Length);       // luego los datos reales
        }


        // Método para leer de un NetworkStream los datos de un objeto Carretera
        public static Carretera LeerDatosCarreteraNS(NetworkStream NS)
        {
            byte[] bufferLongitud = new byte[4];
            int leidos = 0;
            while (leidos < 4)
            {
                int read = NS.Read(bufferLongitud, leidos, 4 - leidos);
                if (read == 0) throw new IOException("Conexión cerrada al leer longitud.");
                leidos += read;
            }

            int longitudMensaje = BitConverter.ToInt32(bufferLongitud, 0);

            byte[] bufferDatos = new byte[longitudMensaje];
            int totalLeidos = 0;
            while (totalLeidos < longitudMensaje)
            {
                int read = NS.Read(bufferDatos, totalLeidos, longitudMensaje - totalLeidos);
                if (read == 0) throw new IOException("Conexión cerrada al leer datos.");
                totalLeidos += read;
            }

            return Carretera.BytesACarretera(bufferDatos);
        }


        //Método para enviar datos de tipo Vehiculo en un NetworkStream
        public static void  EscribirDatosVehiculoNS(NetworkStream NS, Vehiculo V)
        {            
            byte[] datos = V.VehiculoaBytes();
            NS.Write(datos, 0, datos.Length);
        }

        //Método para leer de un NetworkStream los datos de un objeto Vehiculo
        public static Vehiculo LeerDatosVehiculoNS(NetworkStream NS)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] buffer = new byte[1024];
                int bytesLeidos;

                // Leer mientras haya datos disponibles
                do
                {
                    bytesLeidos = NS.Read(buffer, 0, buffer.Length);
                    ms.Write(buffer, 0, bytesLeidos);
                }
                while (NS.DataAvailable);

                byte[] datos = ms.ToArray();
                return Vehiculo.BytesAVehiculo(datos);
            }
        }

        //Método que permite leer un mensaje de tipo texto (string) de un NetworkStream
        public static string LeerMensajeNetworkStream (NetworkStream NS)
        {
            byte[] bufferLectura = new byte[1024];

            //Lectura del mensaje
            int bytesLeidos = 0;
            var tmpStream = new MemoryStream();
            byte[] bytesTotales; 
            do
            {
                int bytesLectura = NS.Read(bufferLectura,0,bufferLectura.Length);
                tmpStream.Write(bufferLectura, 0, bytesLectura);
                bytesLeidos = bytesLeidos + bytesLectura;
            }while (NS.DataAvailable);

            bytesTotales = tmpStream.ToArray();            

            return Encoding.Unicode.GetString(bytesTotales, 0, bytesLeidos);                 
        }

        //Método que permite escribir un mensaje de tipo texto (string) al NetworkStream
        public static void  EscribirMensajeNetworkStream(NetworkStream NS, string Str)
        {            
            byte[] MensajeBytes = Encoding.Unicode.GetBytes(Str);
            NS.Write(MensajeBytes,0,MensajeBytes.Length);                        
        }                          

    }
}
