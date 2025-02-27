using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace MovieReservation.OpenApiTransformers
{
    public class JwtBearerSecurityRequirementTransformer : IOpenApiOperationTransformer
    {
        private record ParsedDisplayName(string NamespaceName, string ControllerName, string ActionName);
        private readonly Dictionary<string, Type> _controllerTypes;
        private readonly IAuthenticationSchemeProvider _schemeProvider;

        public JwtBearerSecurityRequirementTransformer(IAuthenticationSchemeProvider schemeProvider)
        {
            _schemeProvider = schemeProvider;

            Type controllerBaseType = typeof(ControllerBase);

            _controllerTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsClass && t.IsSubclassOf(controllerBaseType))
                .ToDictionary(t => t.Name);
        }

        public async Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
        {
            var schemes = await _schemeProvider.GetSchemeAsync("Bearer");
            if (schemes is null) return;

            string? name = context.Description.ActionDescriptor.DisplayName;
            if (name is null) return;

            ParsedDisplayName displayName = ParseDisplayName(name);

            if (!_controllerTypes.TryGetValue(displayName.ControllerName, out Type? controllerType))
            {
                return;
            }

            var controllerAuthAttributes = controllerType.GetCustomAttributes()
                .OfType<AuthorizeAttribute>();

            var endpointAuthAttributes = controllerType.GetMethod(displayName.ActionName)
                ?.GetCustomAttributes()
                .OfType<AuthorizeAttribute>()
                .Concat(controllerAuthAttributes) ?? controllerAuthAttributes;

            if (!endpointAuthAttributes.Any()) return;

            const string SecurityDefinitionId = "Bearer";

            operation.Responses.Add(StatusCodes.Status403Forbidden.ToString(), new OpenApiResponse
            {
                Description = "User does not have required role or token is invalid."
            });

            operation.Responses.TryAdd(StatusCodes.Status401Unauthorized.ToString(), new OpenApiResponse
            {
                Description = "The access token has not been provided in \"Authorization\" header."
            });

            var requirementSecurityScheme = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = SecurityDefinitionId,
                    Type = ReferenceType.SecurityScheme
                }
            };

            operation.Security.Add(new OpenApiSecurityRequirement
            {
                { requirementSecurityScheme, Array.Empty<string>() }
            });
        }

        private static ParsedDisplayName ParseDisplayName(string name)
        {
            string[] tokenizedName = name.Split(['.', ' ']);

            int typeIndex = 0;
            for (int i = 0; i < tokenizedName.Length; i++)
            {
                if (tokenizedName[i].EndsWith("Controller"))
                {
                    typeIndex = i;
                    break;
                }
            }

            return new ParsedDisplayName(
                NamespaceName: string.Join('.', tokenizedName, 0, typeIndex),
                ControllerName: tokenizedName[typeIndex], 
                ActionName: tokenizedName[typeIndex + 1]);
        }
    }
}
