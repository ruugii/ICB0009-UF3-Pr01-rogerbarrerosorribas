using System.Net.Sockets;

namespace Servidor;

public class Client
{
    public int Id { get; set; }
    public TcpClient TcpClient { get; set; }
    public NetworkStream Stream { get; set; }

    public Client(int id, TcpClient tcpClient)
    {
        Id = id;
        TcpClient = tcpClient;
        Stream = tcpClient.GetStream();
    }
}
