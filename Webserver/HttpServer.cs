using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Webserver
{
	//This class takes care of the whole webserver. 
	public class HttpServer
	{
		HttpListener listener = new HttpListener();
		HttpListenerContext context;
		HttpListenerRequest request;
		HttpListenerResponse response;
		private bool running = true;

		private string url = "http://localhost:8000/";

		public void SimpleHttpListener()
		{
			// Setting a prefix to the listener, in this specific example, it only takes "http://localhost:8000/" coming from the
			// url variable. But it could also be a list of several urls.
			listener.Prefixes.Add(url);

			while (running)
			{
				listener.Start();
				Console.WriteLine("Listening on port 8000");

				context = listener.GetContext();

				if (context != null)
				{
					request = context.Request;
					response = context.Response;
				}

				GetResponse(request, response);
			}

			//The listener/web server is closed when the while loop is no longer true, in this case, the loop ends when a
			// POST request with the abosolute url path is http://localhost:8000/close and the listener will stop.
			listener.Stop();
			Console.WriteLine("Listener is closed.");

		}

		//Method taking care of the response. First et looks on which request is is, more specific which method, afterwards it looks
		// at the url path being requested, and sends a response on the specific request.
		public void GetResponse(HttpListenerRequest request, HttpListenerResponse response)
		{
			string responseContent;
			string responseString = "";

			if (request != null)
			{
				switch (request.HttpMethod)
				{
					case "GET":
						 if (request.Url.AbsolutePath == "/index.html")
						{
							response.StatusCode = (int)HttpStatusCode.OK;
						} else
						{
							response.StatusCode = (int)HttpStatusCode.NotImplemented;
						}

						break;
					case "POST":
						if (request.Url.AbsolutePath == "/close")
						{
							response.StatusDescription = "OK - Closing down the webserver.";
							running = false;
						} else {
							response.StatusCode = (int)HttpStatusCode.NotImplemented;
						}
			
						break;
					case "PUT":
						if ( request.Url.AbsolutePath == "/teaon")
						{
							response.StatusCode = 418;
							response.StatusDescription = "I'm a teapot ... I'm on my way, put the kettle over.";
						}

						break;
					case "DELETE":
						response.StatusCode = (int)HttpStatusCode.Unauthorized;

						break;
					default:
						response.StatusCode = (int)HttpStatusCode.NotImplemented;
						
						break;
				}
			}
			
			//Response string is made
				responseContent = $"{request.HttpMethod} - status code: {response.StatusCode} - {response.StatusDescription}";
			if (responseContent != null)
			{
				responseString = $"<HTML><BODY> {responseContent}</BODY></HTML>";
			}
				
			
			byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

			//Sets the content type to text/html which takes care of showing the actual text and not all the html tags inside it
			response.ContentType = "text/html";

			// Get a response stream and write the response to it.
			response.ContentLength64 = buffer.Length;
			System.IO.Stream output = response.OutputStream;
			output.Write(buffer, 0, buffer.Length);

			// Closing the output stream again after sending the response
			output.Close();
		}
	}
}
