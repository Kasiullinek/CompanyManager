using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace SharedLibrary.Dtos.GenericsModel
{
    public static class Generics
    {
        public static ClaimsPrincipal SetClaimPrincipal(UserSession userSession)
        {
            return new ClaimsPrincipal(new ClaimsIdentity(
                new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, userSession.Id!),
                    new(ClaimTypes.Name, userSession.Name!),
                    new(ClaimTypes.Email, userSession.Email!),
                    new(ClaimTypes.Role, userSession.Role!),
                }, "JwtAuth"));
        }

        public static UserSession GetClaimsFromToken(string jwtToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtToken);
            var claims = token.Claims;

            string Id = claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value!;
            string Name = claims.First(c => c.Type == ClaimTypes.Name).Value!;
            string Email = claims.First(c => c.Type == ClaimTypes.Email).Value!;
            string Role = claims.First(c => c.Type == ClaimTypes.Role).Value!;

            return new UserSession(Id, Name, Email, Role);
        }

        public static JsonSerializerOptions JsonOptions()
        {
            return new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                UnmappedMemberHandling = System.Text.Json.Serialization.JsonUnmappedMemberHandling.Skip,
            };
        }
        public static string SerializeObj<T>(T modelObject) => JsonSerializer.Serialize(modelObject, JsonOptions());

        public static T DeserializeJsonString<T>(string jsonString) => JsonSerializer.Deserialize<T>(jsonString, JsonOptions())!;

        public static IList<T> DeserializeJsonStringList<T>(string jsonString) => JsonSerializer.Deserialize<IList<T>>(jsonString, JsonOptions())!;

        public static StringContent GenerateStringContent(string serializedObj) => new(serializedObj, Encoding.UTF8, "application/json");
    }
}

