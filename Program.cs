using System.Net;

namespace SimpServer;

class Program
{
    static object threadLock = new ();

    static void Main(string[] args)
    {
        string sig = @"
 ___ _                                     
/ __(_)_ __  _ __                          
\__ \ | '  \| '_ \___                      
|___/_|_|_|_| .__/ __| ___ _ ___ _____ _ _ 
            |_|  \__ \/ -_) '_\ V / -_) '_|
                |___/\___|_|  \_/\___|_|  
                                            ";

        IPAddress ip = IPAddress.Loopback;
        int port = 2000;
        bool useHttps = false;
        string? dir = null;

        bool host = true;

        try
        {
            if(args.Length == 0)
            {
                WriteColored("Usage : host -lan -port [any available port] -usehttps -dir [path of directory to host]",ConsoleColor.Green);
                return;
            }

            if (args[0].ToLower() == "host")
            {
                for (int i = 1; i < args.Length; i++)
                {
                    switch (args[i].ToLower())
                    {
                        case "-lan":
                            IPAddress[] addrs = Dns.GetHostAddresses(Dns.GetHostName());
                            foreach(var addr in addrs)
                            {
                                if(addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                                    ip = addr;
                            }
                            if(ip.ToString() == "127.0.0.1")
                                throw new Exception("Not connected to LAN");
                        break;

                        case "-port":
                            try{port = int.Parse(args[i++ + 1]); if(port <= IPEndPoint.MinPort  || port >= IPEndPoint.MaxPort)throw new Exception("Port must be between "+IPEndPoint.MinPort+" and "+IPEndPoint.MaxPort);}catch{throw new Exception("Invalid port number");}
                        break;

                        case "-usehttps":
                            useHttps = true;
                        break;

                        case "-dir":
                            if (dir != null)
                            {
                                throw new Exception("Redeclaring host directory");
                            }
                            else
                            {
                                if (Directory.Exists(args[i + 1]) && File.Exists(args[i + 1] + "/index.html"))
                                    dir = args[i++ + 1];
                                else
                                    throw new Exception("The specified directory does not have an index.html file");
                            }
                        break;

                        default:
                            throw new Exception("Unidentified command used \"" + args[i] + "\"");
                    }
                }

                if (dir == null)
                    throw new Exception("Host directory not specified");
            }
            else
            {
                throw new Exception("Unidentified command used \"" + args[0] + "\"");
            }
        }
        catch(Exception e)
        {
            WriteColored("Error : " + e.Message,ConsoleColor.Red);
            WriteColored("Usage : host -lan -port [any available port] -usehttps -dir [path of directory to host]",ConsoleColor.Green);
            host = false;
        }

        if(host)
        {
            WriteColored(sig,ConsoleColor.Blue);

            WriteColored($"Hosting {ip}:{port}",ConsoleColor.Cyan);

            Server s = new Server(ip,port,useHttps,dir);

            WriteColored("To stop, enter stop",ConsoleColor.Yellow);
            bool stop = false;
            do
            {
                var l = Console.ReadLine();

                if(l == "stop")
                {
                    s.Shutdown();
                    stop = true;
                }
            }while(!stop);
        }
    }

    public static void WriteColored(string line,ConsoleColor color,ConsoleColor? backColor = null)
    {
        lock(threadLock)
        {
            if(backColor != null)
                Console.BackgroundColor = (ConsoleColor)backColor;
                
            Console.ForegroundColor = color;
            Console.WriteLine(line);
            Console.ResetColor();
        }
    }
}