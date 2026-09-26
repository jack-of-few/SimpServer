using System.Net;

namespace SimpServer;

class Program
{
    static object threadLock = new();

    static string sig = @"
 ___ _                                     
/ __(_)_ __  _ __                          
\__ \ | '  \| '_ \___                      
|___/_|_|_|_| .__/ __| ___ _ ___ _____ _ _ 
            |_|  \__ \/ -_) '_\ V / -_) '_|
                |___/\___|_|  \_/\___|_|  
                                            ";

    public enum ConsoleMessageType
    {
        Question,
        Assert,
        Error,
        Title
    }

    public static void WriteColored(string line, ConsoleMessageType messageType)
    {
        lock (threadLock)
        {
            switch (messageType)
            {
                case ConsoleMessageType.Question:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;

                case ConsoleMessageType.Assert:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;

                case ConsoleMessageType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;

                case ConsoleMessageType.Title:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
            }

            Console.WriteLine(line);
            Console.ResetColor();
        }
    }

    static void Main(string[] args)
    {
        WriteColored("Enter directory path :", ConsoleMessageType.Question);
        string? dir = Console.ReadLine();
        while (!Directory.Exists(dir))
        {
            WriteColored("The given directory does not exist. Please provide another path : ", ConsoleMessageType.Question);
            dir = Console.ReadLine();
        }

        IPAddress ip = IPAddress.Loopback;
        WriteColored("Host the directory on -\n1. This PC\n2. LAN", ConsoleMessageType.Question);
        ConsoleKeyInfo key;
        do
        {
            key = Console.ReadKey();
        } while (key.Key != ConsoleKey.D1 && key.Key != ConsoleKey.D2);
        if (key.Key == ConsoleKey.D2)
        {
            bool connected = false;
            do
            {
                IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());
                foreach (var ipAddr in ips)
                {
                    if (ipAddr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        ip = ipAddr;
                }

                if (ip.Equals(IPAddress.Loopback))
                {
                    WriteColored("You are not connected to any network. Connect and try again.", ConsoleMessageType.Error);
                    Console.ReadKey();
                }
                else
                {
                    connected = true;
                }
            } while (!connected);
        }

        Random r = new();
        int port = r.Next(10000, 20000);

        Server s = new(ip, port, dir);

        Console.Clear();
        WriteColored(sig, ConsoleMessageType.Title);
        WriteColored($"Hosting http://{ip}:{port}", ConsoleMessageType.Assert);
        WriteColored("To clear these logs, enter clear", ConsoleMessageType.Assert);
        WriteColored("To stop, enter stop", ConsoleMessageType.Assert);

        bool stop = false;
        do
        {
            var l = Console.ReadLine();

            if (l == "stop")
            {
                s.Shutdown();
                stop = true;
            }
            if (l == "clear")
            {
                Console.Clear();
                WriteColored(sig, ConsoleMessageType.Title);
                WriteColored($"Hosting http://{ip}:{port}", ConsoleMessageType.Assert);
                WriteColored("To clear these logs, enter clear", ConsoleMessageType.Assert);
                WriteColored("To stop, enter stop", ConsoleMessageType.Assert);
            }
        } while (!stop);
    }
}