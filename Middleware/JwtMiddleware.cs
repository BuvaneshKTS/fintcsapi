using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using FintcsApi.Data;

namespace FintcsApi.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;

        public JwtMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _config = config;
        }

        public async Task Invoke(HttpContext context, AppDbContext dbContext)
        {
            var token = context.Request.Headers["Authorization"]
                .FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                await AttachUserToContext(context, dbContext, token);
            }

            await _next(context);
        }

        private async Task AttachUserToContext(HttpContext context, AppDbContext dbContext, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? "your-super-secret-jwt-key-that-should-be-at-least-32-characters");
                
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _config["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _config["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "UserId").Value);

                // Attach user to context
                var user = await dbContext.Users.FindAsync(userId);
                context.Items["User"] = user;
            }
            catch (Exception ex)
            {
                // Token validation failed - do nothing if jwt validation fails
                // User will not be attached to context and will be treated as unauthenticated
                Console.WriteLine($"JWT validation failed: {ex.Message}");
            }
        }
    }
}
