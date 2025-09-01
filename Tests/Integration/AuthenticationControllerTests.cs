using ApplicationLogic.ViewModels;
using System.Net;
using System.Net.Http.Json;

namespace Tests.Integration
{
    [Collection("Integration")]
    public class AuthenticationControllerTests
    {
        private readonly MovieReservationWebApplicationFactory _factory;

        public AuthenticationControllerTests(MovieReservationWebApplicationFactory factory)
        {
            _factory = factory;
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
    }
}