using System;
using IdentityModel.Client;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace OxClient
{
    public class Program
    {
        public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync()
        {
            // Discover endpoints from metadata
            // IdentityModel includes a client library to use with the discovery endpoint. 
            // This way you only need to know the base-address of IdentityServer - the actual endpoint addresses can be read from the metadata.
            var disco = await DiscoveryClient.GetAsync("http://localhost:5000");

            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);

                return;
            }

            // Request token
            // Next you can use the TokenClient class to request the token. To create an instance you need to pass in the token endpoint address, client id and secret.
            // Next you can use the RequestClientCredentialsAsync method to request a token for your API.
            var tokenClient = new TokenClient(disco.TokenEndpoint, "ro.client", "secret");
            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("alice", "password", "api1");

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);

                return;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            // call api
            // The last step is now to call the API.
            // To send the access token to the API you typically use the HTTP Authorization header.This is done using the SetBearerToken extension method:
            var client = new HttpClient();

            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.GetAsync("http://localhost:5001/identity");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();

                Console.WriteLine(JArray.Parse(content));
            }
        }
    }
}
