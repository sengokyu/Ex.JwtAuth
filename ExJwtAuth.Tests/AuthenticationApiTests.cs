using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Json;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace ExJwtAuth
{
    public class AuthenticationApiTests
        : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> factory;

        public AuthenticationApiTests(WebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
        }

        [Fact]
        public async Task Test未認証だと401()
        {
            // Given
            var uri = "/";

            using (var client = factory.CreateClient())
            {
                // When
                var response = await client.GetAsync(uri);

                // Then
                response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            }
        }

        [Fact]
        public async Task Testユーザ名をPOSTするとJWTが返る()
        {
            // Given
            var uri = "/";
            var jsonContent = new Helpers.JsonContent(new
            {
                username = "yamada takao",
                password = "hi mi tu",
            });

            using (var client = factory.CreateClient())
            {
                // When
                var response = await client.PostAsync(uri, jsonContent);

                // Then
                response.IsSuccessStatusCode.Should().BeTrue();

                var content = await response.Content.ReadAsStringAsync();
                var actual = JToken.Parse(content);
                actual.Should().HaveElement("token");
            }
        }

        [Fact]
        public async Task TestJWTで認証が通ると名前が返る()
        {
            // Given
            var uri = "/";
            var username = "yamada tarou";
            var token = await Login(username);
            var authenticationHeader = new AuthenticationHeaderValue(
                "Bearer", token
            );

            using (var client = factory.CreateClient())
            {
                // When
                client.DefaultRequestHeaders.Authorization = authenticationHeader;
                var response = await client.GetAsync(uri);

                // Then
                response.IsSuccessStatusCode.Should().BeTrue();

                var content = await response.Content.ReadAsStringAsync();
                var actual = JToken.Parse(content);
                var expected = JToken.FromObject(new
                {
                    username = username
                });
                actual.Should().BeEquivalentTo(expected);
            }
        }

        [Fact]
        public async Task Test適当なJWTでは401が返る()
        {
            // Given
            var uri = "/";
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";
            var authenticationHeader = new AuthenticationHeaderValue(
                "Bearer", token
            );

            using (var client = factory.CreateClient())
            {
                // When
                var response = await client.GetAsync(uri);

                // Then
                response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            }
        }

        private async Task<string> Login(string username)
        {
            var uri = "/";
            var jsonContent = new Helpers.JsonContent(new
            {
                username = username,
                password = "hi mi tu",
            });

            using (var client = factory.CreateClient())
            {
                var response = await client.PostAsync(uri, jsonContent);
                var content = await response.Content.ReadAsStringAsync();
                var jtoken = JToken.Parse(content);

                return jtoken.Value<string>("token");
            }
        }
    }
}