using System.Net;
using Webserver;

internal class Program

{
	static void Main(string[] args)
	{
		HttpServer server = new HttpServer();
		server.SimpleHttpListener();

	}


}


