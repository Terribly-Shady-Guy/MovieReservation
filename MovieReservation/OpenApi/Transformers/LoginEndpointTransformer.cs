using Microsoft.AspNetCore.OpenApi;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using MovieReservation.Services;
using MovieReservation.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MovieReservation.OpenApi.Transformers
{
    public class LoginEndpointTransformer : IOpenApiOperationTransformer
    {
        private readonly IAuthenticationTokenProvider _tokenProvider;
        public LoginEndpointTransformer(IAuthenticationTokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }

        public async Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
        {
            var requestExample = new OpenApiObject()
            {
                ["username"] = new OpenApiString("johndoe"),
                ["password"] = new OpenApiString("Password123@")
            };

            foreach (var mediaType in operation.RequestBody.Content)
            {
                mediaType.Value.Schema.Example = requestExample;
            }

            if (operation.Responses.TryGetValue(StatusCodes.Status200OK.ToString(), out var response))
            {
                await AddResponseExample(response);
            }
        }

        private async Task AddResponseExample(OpenApiResponse response)
        {
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Role, "User"));

            AuthenticationToken token = await _tokenProvider.GenerateTokens(identity);

            var responseExample = new OpenApiObject()
            {
                ["accessToken"] = new OpenApiString(token.AccessToken),
                ["refreshToken"] = new OpenApiString(token.RefreshToken),
                ["refreshExpiration"] = new OpenApiDateTime(token.RefreshExpiration)
            };

            foreach (var mediaType in response.Content)
            {
                mediaType.Value.Schema.Example = responseExample;
            }
        }
    }
}
