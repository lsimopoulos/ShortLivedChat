using System.Collections.Generic;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace ShortLivedChatServer.IdentityServerConfig
{
    /// <summary>
    /// Configuration class for Identity Server.
    /// </summary>
    public static class ISConfig
    {
        private static List<TestUser> _registeredUsers = new List<TestUser>{
            new TestUser
            {
                SubjectId = "1",
                Username = "bob",
                Password = "password"
            },
            new TestUser
            {
                SubjectId = "2",
                Username = "mrC",
                Password = "pass"
            }};

        /// <summary>
        ///     Get api resources.
        /// </summary>
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("shortlivedchat", "Short lived Chat")
            };
        }

        /// <summary>
        ///     Get clients.
        /// </summary>
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "chat_console_client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    ClientSecrets =
                    {
                        new Secret("superdupersecret".Sha256())
                    },
                    AllowedScopes = {"shortlivedchat"}
                },
                new Client
                {
                    ClientId = "chat_web_client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    ClientSecrets =
                    {
                        new Secret("websuperdupersecret".Sha256())
                    },
                    AllowedScopes = {"shortlivedchat"}
                }
            };
        }

        /// <summary>
        ///     Get users.
        /// </summary>
        public static List<TestUser> GetUsers()
        {
            return _registeredUsers;
        }

        public static void AddUser(TestUser user)
        {
            user.SubjectId = (_registeredUsers.Count + 1).ToString();
            _registeredUsers.Add(user);
        }
    }
}