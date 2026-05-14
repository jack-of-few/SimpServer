using System.Text;
using SimpServer;

static class HTTPParser
{
    public static string Parse(string s)//check if it is http get req, and if it is then extract and return the resource url requested
    {
        string[] lines = s.Split(new char [] {'\r','\n'},StringSplitOptions.RemoveEmptyEntries);

        if(lines[0].StartsWith("GET") && lines[0].EndsWith("HTTP/1.1"))
            return lines[0].Substring(4,lines[0].Length-13);
        else
            return "";
    }

    public static byte[] CreateResponse(string resUrl,string rootDir)//returns full http response in byte[]
    {
        string contentType = "";

        switch(resUrl.Split(".")[^1])
        {
            case "html":
            contentType = "text/html";
            break;

            case "js":
            contentType = "text/javascript";
            break;

            case "css":
            contentType = "text/css";
            break;

            case "png":
            contentType = "image/png";
            break;

            case "jpeg":
            contentType = "image/jpeg";
            break;

            case "svg":
            contentType = "image/svg+xml";
            break;
        }
        
        if(File.Exists(resUrl) /*&& contentType != ""*/ && Path.GetFullPath(resUrl).Contains(Path.GetFullPath(rootDir)))//check if file exists and is inside root dir folder and has supported mime type
        {
            byte[] data = File.ReadAllBytes(resUrl);

            byte[] header = Encoding.UTF8.GetBytes($"HTTP/1.1 200 OK\nContent-Type: {contentType}; charset=UTF-8\nContent-Length: {data.Length}\n\n");
            return header.Concat(data).ToArray();
        }
        else
        {
            byte[] data = Encoding.UTF8.GetBytes("<!doctype html><head><title>404 not found</title></head></html>");
            byte[] header = Encoding.UTF8.GetBytes($"HTTP/1.1 404 Not Found\nContent-Type: text/html; charset=UTF-8\nContent-Length: {data.Length}\n\n");
            Program.WriteColored($"Requested resource \"{resUrl}\" not found",ConsoleColor.Red);
            return header.Concat(data).ToArray();
        }
    }
}