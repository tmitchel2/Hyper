using System.Net;
using System.Threading.Tasks;
using System.Web.Http.SelfHost;
using Hyper;
using Hyper.Http.Formatting;
using HyperTests.Models;
using NUnit.Framework;

namespace HyperTests
{
    [TestFixture]
    public class WebTests
    {
        [Test]
        public void SimpleTest()
        {
            var task = SimpleTestAsync();
            task.Wait();
        }

        public async Task SimpleTestAsync()
        {
            var url = @"http://localhost:8352";
            var server = GetServer(url);
            server.OpenAsync().Wait();

            using (var client = GetClient())
            {
                // Get the app api
                var api = await client.Get<Api>(url);
                
                // Login
                var session = await api.Sessions.Post(new Session { Username = "username", Password = "password" }, client);

                // Logout
                await session.Delete(client, HttpStatusCode.Unauthorized);
            }

            await server.CloseAsync();
        }

        private static HttpSelfHostServer GetServer(string url)
        {
            var config = Application.GetConfiguration(url, false);
            return new HttpSelfHostServer(config);
        }

        private static HyperClient GetClient()
        {
            var config = new HyperClientConfiguration();
            config.Formatters.Add(new HyperMediaTypeFormatter("json", new JsonMediaTypeFormatter(), Application.GetTypes()));
            return new HyperClient(config);
        }
    }
}