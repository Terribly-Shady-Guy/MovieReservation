using ApplicationLogic.ViewModels;
using System.Net;
using System.Net.Http.Json;
using Tests.IntegrationInfrastructure;

namespace Tests.Integration
{
    public class AuthenticationControllerTests : IntegrationTestBase
    {
        private readonly CancellationToken _token = TestContext.Current.CancellationToken;

        public AuthenticationControllerTests(MovieReservationWebApplicationFactory factory) : base(factory)
        { }

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
            
            HttpResponseMessage response = await client.PostAsJsonAsync("/api/v1/Authentication/Login", loginModel, _token);

            Assert.Equal(expectedStatus, response.StatusCode);
        }

        [Fact]
        public async Task Workflow_AuthenticationLifecycle_Succeeds()
        {
            HttpClient client = _factory.CreateClient();

            var loginModel = new UserLoginDto
            {
                Email = "root@example.com",
                Password = "Admin246810@",
            };

            HttpResponseMessage loginResponse = await client.PostAsJsonAsync("/api/v1/authentication/login", loginModel, _token);

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var token = await loginResponse.Content.ReadFromJsonAsync<AuthenticationToken>(_token);
            Assert.NotNull(token);

            var refreshBody = new AuthenticationTokenRequestBody
            {
                AccessToken = token.AccessToken,
                RefreshToken = token.RefreshToken
            };

            HttpResponseMessage refreshResponse = await client.PatchAsJsonAsync("/api/v1/authentication", refreshBody, _token);
            Assert.Equal(HttpStatusCode.OK, refreshResponse.StatusCode);

            token = await refreshResponse.Content.ReadFromJsonAsync<AuthenticationToken>(_token);
            Assert.NotNull(token);

            var logoutRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/authentication?refreshToken={token.RefreshToken}");

            logoutRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token.AccessToken);

            HttpResponseMessage logoutResponse = await client.SendAsync(logoutRequest, _token);
            Assert.Equal(HttpStatusCode.NoContent, logoutResponse.StatusCode);
        }
    }
}