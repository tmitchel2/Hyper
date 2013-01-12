using System.Net;
using System.Threading.Tasks;
using System.Web.Http.SelfHost;
using Hyper;
using HyperTests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HyperTests
{
    [TestClass]
    public class WebTests
    {
        [TestMethod]
        public async Task HelperWebTest()
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
            return new HyperClient(Application.GetTypes());
        }
    }
}