using ApplicationLogic.ViewModels;
using System.Net;
using System.Net.Http.Json;
using Tests.IntegrationInfrastructure;

namespace Tests.Integration
{
    [Collection<WebApplicationFactoryCollectionFixture>]
    public class AuthenticationControllerTests : IAsyncDisposable
    {
        private readonly MovieReservationWebApplicationFactory _factory;

        public AuthenticationControllerTests(MovieReservationWebApplicationFactory factory)
        {
            _factory = factory;
        }

        public async ValueTask DisposeAsync()
        {
            await _factory.ResetDb(TestContext.Current.CancellationToken);
        }

        [Theory]
        [InlineData("root@example.com", "Admin246810@", HttpStatusCode.OK, TestDisplayName = "Successful login")]
        [InlineData("root@example.com", "SomePassword1@", HttpStatusCode.Unauthorized, TestDisplayName = "Failed with wrong password")]
        [InlineData("notauser@example.com", "NotAUser1@", HttpStatusCode.Unauthorized, TestDisplayName = "Failed with nonexistent user")]
        [InlineData("notanemail", "ValidPassword1@", HttpStatusCode.BadRequest, TestDisplayName = "Failed with invalid email string")]
        public async Task Post_LoginHandler_ReturnsExpectedStatus(string email, string password, HttpStatusCode expectedStatus)
        {
            HttpClient client = _factory.CreateClient();
            var loginModel = new UserLoginDto
            {
                Email = email,
                Password = password
            };
            
            HttpResponseMessage response = await client.PostAsJsonAsync("/api/v1/Authentication/Login", loginModel, TestContext.Current.CancellationToken);

            Assert.Equal(expectedStatus, response.StatusCode);
        }

        [Fact]
        public async Task Workflow_Authentication_Succeeds()
        {
            HttpClient client = _factory.CreateClient();

            var loginModel = new UserLoginDto
            {
                Email = "root@example.com",
                Password = "Admin246810@",
            };

            CancellationToken cancellationToken = TestContext.Current.CancellationToken;

            HttpResponseMessage loginResponse = await client.PostAsJsonAsync("/api/v1/authentication/login", loginModel, cancellationToken);

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var token = await loginResponse.Content.ReadFromJsonAsync<AuthenticationToken>(cancellationToken);
            Assert.NotNull(token);

            var refreshBody = new AuthenticationTokenRequestBody
            {
                AccessToken = token.AccessToken,
                RefreshToken = token.RefreshToken
            };

            HttpResponseMessage refreshResponse = await client.PatchAsJsonAsync("/api/v1/authentication", refreshBody, cancellationToken);
            Assert.Equal(HttpStatusCode.OK, refreshResponse.StatusCode);

            token = await refreshResponse.Content.ReadFromJsonAsync<AuthenticationToken>(cancellationToken);
            Assert.NotNull(token);

            var logoutRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/authentication?refreshToken={token.RefreshToken}");

            logoutRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token.AccessToken);

            HttpResponseMessage logoutResponse = await client.SendAsync(logoutRequest, cancellationToken);
            Assert.Equal(HttpStatusCode.NoContent, logoutResponse.StatusCode);
        }
    }
}