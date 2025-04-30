using System.Net.Sockets;

namespace Servidor;

public class Client
{
  public int Id { get; set; }
  public NetworkStream Stream { get; set; }

  public Client(int id, NetworkStream stream)
  {
    Id = id;
    Stream = stream;
  }
}
