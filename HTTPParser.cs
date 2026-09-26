using System.Text;

static class HTTPParser
{
    public static string ParseRequest(string s)//check if it is http get req, and if it is then extract and return the resource url requested
    {
        string[] lines = s.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        if (lines[0].StartsWith("GET") && lines[0].EndsWith("HTTP/1.1"))
            return lines[0].Substring(4, lines[0].Length - 13);
        else
            return "";
    }

    public static byte[] CreateResponse(string resUrl, string rootDir, out bool success)//returns full http response in byte[]
    {
        if (File.Exists(resUrl) && Path.GetFullPath(resUrl).Contains(Path.GetFullPath(rootDir)))//check if file exists and is inside root dir folder
        {
            byte[] data = File.ReadAllBytes(resUrl);

            byte[] header;

            header = Encoding.UTF8.GetBytes($"HTTP/1.1 200 OK\nConnection: keep-alive\nContent-Type: ;charset=UTF-8\nContent-Length: {data.Length}\n\n");

            success = true;
            return header.Concat(data).ToArray();
        }
        else
        {
            byte[] data = Encoding.UTF8.GetBytes("<!doctype html><head><title>404 not found</title></head><body>invalid url</body></html>");
            byte[] header = Encoding.UTF8.GetBytes($"HTTP/1.1 404 Not Found\nConnection : keep-alive\nContent-Type: text/html; charset=UTF-8\nContent-Length: {data.Length}\n\n");
            success = false;
            return header.Concat(data).ToArray();
        }
    }
}