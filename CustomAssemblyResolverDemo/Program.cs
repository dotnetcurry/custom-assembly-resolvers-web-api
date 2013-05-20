using System;
using System.Net.Http;
using System.ServiceModel;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.SelfHost;


namespace CustomAssemblyResolverDemo
{
    class Program
    {
        static readonly Uri _baseAddress = new Uri("http://localhost:60064");

        static void Main(string[] args)
        {
            HttpSelfHostServer server = null;
            try
            {
                // Set up server configuration
                HttpSelfHostConfiguration config = new HttpSelfHostConfiguration(_baseAddress);
                config.HostNameComparisonMode = HostNameComparisonMode.Exact;
                config.Routes.MapHttpRoute(
                 name: "DefaultApi",
                 routeTemplate: "api/{controller}/{id}",
                 defaults: new { id = RouteParameter.Optional }
                );
                // Set our own assembly resolver where we add the assemblies we need           
                DynamicAssemblyResolver assemblyResolver = new DynamicAssemblyResolver();
                config.Services.Replace(typeof(IAssembliesResolver), assemblyResolver);
                // Create server
                server = new HttpSelfHostServer(config);
                // Start listening
                server.OpenAsync().Wait();
                Console.WriteLine("Listening on " + _baseAddress);
                while (true)
                {
                    // Run HttpClient issuing requests
                    RunDynamicClientAsync();
                    Console.WriteLine("Press Ctrl+C to exit...");
                    Console.ReadLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not start server: {0}", e.GetBaseException().Message);
                Console.WriteLine("Hit ENTER to exit...");
                Console.ReadLine();
            }
            finally
            {
                if (server != null)
                {
                    // Stop listening
                    server.CloseAsync().Wait();
                }
            }
        }

        static async void RunDynamicClientAsync()
        {
            // Create an HttpClient instance
            HttpClient client = new HttpClient();

            // Send GET request to server for the hello controller which lives in the controller library
            Uri address = new Uri(_baseAddress, "/api/DynamicWebApi");
            HttpResponseMessage response = await client.GetAsync(address);

            // Ensure we get a successful response.
            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();
            // Ensure we get a successful response.
            response.EnsureSuccessStatusCode();

            content = await response.Content.ReadAsStringAsync();
            Console.WriteLine("On The Fly Controller says {0}", content);
        }
    }
}
