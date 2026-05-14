# SimpServer

### About
A .NET CLI to host **static** webpages with configured directory on local network

### Features
1. Lightweight program, no dependency install required
2. Handle multiple clients
3. Cool terminal colors

### How to use?
Simply enter this into your terminal-

```
simpserver host -dir [path of directory to host] -port [network port to use] -lan
```

* Commands can be written in any order
* dir : the webpage directory to serve, must have **index.html**
* port : optional, spcifies the network port to use. Must be between min and max port on system, and should not be in use by another process
* lan : optional flag to make the server visible on LAN. If not used, only the host computer can connect(useful for debugging)

### TODOs
* Add support for hosting multiple servers
* Local domain name support
* Configure server settings and advanced headers
* HTPPs support

### Thanks to + Resources
* C# Network Programming by Richard Blum
* [Asynchronous Programming Model (OBSELETE!)](https://learn.microsoft.com/en-us/dotnet/standard/asynchronous-programming-patterns/asynchronous-programming-model-apm)
* [HTTP Request Structure](https://developer.mozilla.org/en-US/docs/Web/HTTP/Guides/Messages)
* [MDN GET request method](https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Methods/GET)
* [Text to ASCII Art Generator](https://patorjk.com/)