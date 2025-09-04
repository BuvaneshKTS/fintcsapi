using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FintcsApi.Data;
using FintcsApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database configuration - PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// JWT Configuration - use environment variables for security
var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") 
             ?? jwtSettings["Key"] 
             ?? "your-super-secret-jwt-key-that-should-be-at-least-32-characters";
var key = Encoding.UTF8.GetBytes(jwtKey);

// ✅ Configure JWT (disable issuer/audience validation for dev)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),

        // ❌ Disable for devtunnels testing
        ValidateIssuer = false,
        ValidateAudience = false,

        // ✅ Keep lifetime validation
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// ✅ CORS - Allow Angular frontend + DevTunnels
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy
                .SetIsOriginAllowed(origin =>
                {
                    // Allow localhost Angular
                    if (origin == "http://localhost:4200") return true;

                    // Allow all *.devtunnels.ms subdomains
                    try
                    {
                        var host = new Uri(origin).Host;
                        return host.EndsWith("devtunnels.ms");
                    }
                    catch
                    {
                        return false;
                    }
                })
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
});

// Register services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<LoanTakenService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

// ✅ CORS must come before Authentication/Authorization
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

// Redirect root to Swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapControllers();

// Ensure database is created / migrated
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
}

// ✅ Bind to all interfaces (for devtunnels)
app.Run("http://0.0.0.0:5000");
