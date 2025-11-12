using ApplicationLogic.Interfaces;
using ApplicationLogic.ViewModels;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json.Nodes;

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
            if (operation.RequestBody?.Content is not null)
            {
                var requestExample = new JsonObject()
                {
                    ["email"] = "john.doe@example.com",
                    ["password"] = "Password123@"
                };

                foreach (var mediaType in operation.RequestBody.Content)
                {
                    mediaType.Value.Example = requestExample;
                }
            }
            

            if (operation.Responses?.TryGetValue(StatusCodes.Status200OK.ToString(), out var okResponse) ?? false)
            {
                await Add200ResponseExample(okResponse);
            }

            if (operation.Responses?.TryGetValue(StatusCodes.Status202Accepted.ToString(), out var acceptedResponse) ?? false)
            {
                Add202ResponseExample(acceptedResponse);
            }            
        }

        private async Task Add200ResponseExample(IOpenApiResponse response)
        {
            if (response.Content is null)
            {
                return;
            }

            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, Guid.NewGuid().ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Role, "User"));

            AuthenticationToken token = await _tokenProvider.GenerateTokens(identity);

            var responseExample = new JsonObject()
            {
                ["accessToken"] = token.AccessToken,
                ["refreshToken"] = token.RefreshToken,
                ["refreshExpiration"] = token.RefreshExpiration
            };

            foreach (KeyValuePair<string, OpenApiMediaType> mediaType in response.Content)
            {
                mediaType.Value.Example = responseExample;
            }
        }

        private static void Add202ResponseExample(IOpenApiResponse response)
        {
            if (response.Content is null)
            {
                return;
            }

            var messageExample = new JsonObject()
            {
                ["message"] = "some message for result",
                ["userId"] = "some user id string"
            };

            foreach (KeyValuePair<string, OpenApiMediaType> mediaType in response.Content)
            {
                mediaType.Value.Example = messageExample;
            }
        }
    }
}
