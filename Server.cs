using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SimpServer;

class Server
{
    Socket serverSocket;
    public List<Client> clients;
    public IPAddress ipAddress;
    public int port;
    public bool useHttps;
    public string directory;
    public DateTime startTime;
    
    public Server(IPAddress ip,int port, bool https,string dir)
    {
        ipAddress = ip;
        this.port = port;
        useHttps = https;
        directory = dir;
        startTime = DateTime.Now;
        clients = new ();

        serverSocket = new (SocketType.Stream,ProtocolType.Tcp);
        serverSocket.Bind(new IPEndPoint(ip,port));
        serverSocket.Listen();

        serverSocket.BeginAccept(new AsyncCallback(OnClientConnect),null);
    }

    void OnClientConnect(IAsyncResult res)
    {
        serverSocket.BeginAccept(new AsyncCallback(OnClientConnect),null);

        Socket soc = serverSocket.EndAccept(res);
        Client client = new (soc);
        clients.Add(client);

        Program.WriteColored("Client connected at " + DateTime.Now.ToLongTimeString(),ConsoleColor.White);

        soc.BeginReceive(client.buffer,0,client.buffer.Length,SocketFlags.None,new AsyncCallback(OnMessageReceive),client);
    }

    void OnMessageReceive(IAsyncResult res)
    {
        Client cl = (Client)res.AsyncState;

        int byteCount = cl.sock.EndReceive(res);

        if(byteCount > 0)
        {
            string mess = Encoding.UTF8.GetString(cl.buffer,0,byteCount);
            string urlDir = HTTPParser.Parse(mess);

            if(urlDir != "")
            {
                if(urlDir == "/")
                    SendResTo(cl,directory+"/index.html");
                else
                    SendResTo(cl,directory+urlDir);
            }

            cl.sock.BeginReceive(cl.buffer,0,cl.buffer.Length,SocketFlags.None,new AsyncCallback(OnMessageReceive),cl);
        }
    }

    void SendResTo(Client cl,string resUrl)
    {
        byte[] resp = HTTPParser.CreateResponse(resUrl,directory);
          
        cl.sock.Send(resp);
    }

    public void Shutdown()
    {
        foreach(var c in clients)
        {
            c.sock.Close();
        }
        serverSocket.Close();
    }
}