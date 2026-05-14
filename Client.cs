using System.Net.Sockets;

namespace SimpServer;

class Client
{
    public Socket sock;
    public byte[] buffer;

    public Client(Socket s)
    {
        sock = s;
        buffer = new byte[2048];
    }
}