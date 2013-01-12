using System;
using System.Threading.Tasks;
using Hal;
using JsonHalAspNet.Models;
using JsonHalTests.Hal;
using JsonHalTests.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JsonHalTests
{
    [TestClass]
    public class UsageTests
    {
        [TestMethod]
        public async Task CanUseHalClient()
        {
            using (var hal = GetHalClient())
            {
                // Get the api
                // Note : This is done by setting up api object and refreshing it with a GET call
                var api = new Api { Self = new HalLink(@"http://api.example.com") };
                api = await api.Get(hal);

                // Create a new session
                var session =
                    await
                    api.Sessions.Post(
                        new Session { User = new User { Email = @"admin@mywebsite.com", Password = "admin" } }, hal);
                Assert.IsNotNull(session);

                // Create a new user
                var newuser = await api.Users.Post(new User { Email = @"user@email.com", Password = "password" }, hal);
                Assert.IsNotNull(newuser);

                // Get sessions
                var sessions = await api.Sessions.Get(hal);
                Assert.IsNotNull(sessions);

                // Get users
                var users = await api.Users.Get(hal);
                Assert.IsNotNull(users);

                // Update user
                newuser.Password = "newpassword";
                var updatedUser = await newuser.Put(hal);
                Assert.IsNotNull(updatedUser);

                // Delete user
                await updatedUser.Delete(hal);

                // Logout
                await session.Delete(hal);
            }
        }

        private static HalClient GetHalClient()
        {
            var dataTransport = GetTransport();
            var dataSerialiser = new HalSerialiser();
            return new HalClient(dataTransport, dataSerialiser);
        }

        private static HalTransport GetTransport()
        {
            var dataTransport = new HalTransport
                {
                    GetFunc = url =>
                        {
                            switch (url)
                            {
                                case @"http://api.example.com":
                                    return
                                        Task<string>.Factory.StartNew(
                                            () => ResourceUtil.GetResource("JsonHalTests.TestOutput.Api.json"));
                                case @"/sessions":
                                    return
                                        Task<string>.Factory.StartNew(
                                            () => ResourceUtil.GetResource("JsonHalTests.TestOutput.Sessions.json"));
                                case @"/users":
                                    return
                                        Task<string>.Factory.StartNew(
                                            () => ResourceUtil.GetResource("JsonHalTests.TestOutput.Users.json"));
                                default:
                                    throw new NotSupportedException();
                            }
                        },
                    PostFunc = (url, item) =>
                        {
                            switch (url)
                            {
                                case @"/sessions":
                                    return
                                        Task<string>.Factory.StartNew(
                                            () =>
                                            ResourceUtil.GetResource(
                                                "JsonHalTests.TestOutput.Session.json"));
                                case @"/users":
                                    return
                                        Task<string>.Factory.StartNew(
                                            () =>
                                            ResourceUtil.GetResource(
                                                "JsonHalTests.TestOutput.User.json"));
                                default:
                                    throw new NotSupportedException();
                            }
                        },
                    PutFunc = (url, item) =>
                        {
                            switch (url)
                            {
                                case @"/users/0":
                                    return
                                        Task<string>.Factory.StartNew(
                                            () =>
                                            ResourceUtil.GetResource(
                                                "JsonHalTests.TestOutput.UserWithNewPassword.json"));
                                default:
                                    throw new NotSupportedException();
                            }
                        },
                    DeleteFunc = url =>
                        {
                            switch (url)
                            {
                                case @"/users/0":
                                    return new Task(() => { });
                                case @"/sessions/0":
                                    return new Task(() => { });
                                default:
                                    throw new NotSupportedException();
                            }
                        }
                };
            return dataTransport;
        }
    }
}