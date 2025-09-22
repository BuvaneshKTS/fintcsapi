// using Microsoft.EntityFrameworkCore;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.IdentityModel.Tokens;
// using System.Text;
// using FintcsApi.Data;
// using FintcsApi.Services;

// var builder = WebApplication.CreateBuilder(args);

// // Add services to the container.
// builder.Services.AddControllers();
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

// builder.Services.AddControllers()
//     .AddJsonOptions(options =>
//     {
//         options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
//     });

// // Database configuration - PostgreSQL
// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
// );

// // JWT Configuration - use environment variables for security
// var jwtSettings = builder.Configuration.GetSection("Jwt");
// var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
//              ?? jwtSettings["Key"]
//              ?? "your-super-secret-jwt-key-that-should-be-at-least-32-characters";
// var key = Encoding.UTF8.GetBytes(jwtKey);

// // ✅ Configure JWT (disable issuer/audience validation for dev)
// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
// })
// .AddJwtBearer(options =>
// {
//     options.RequireHttpsMetadata = false;
//     options.SaveToken = true;
//     options.TokenValidationParameters = new TokenValidationParameters
//     {
//         ValidateIssuerSigningKey = true,
//         IssuerSigningKey = new SymmetricSecurityKey(key),

//         // ❌ Disable for devtunnels testing
//         ValidateIssuer = false,
//         ValidateAudience = false,

//         // ✅ Keep lifetime validation
//         ValidateLifetime = true,
//         ClockSkew = TimeSpan.Zero
//     };
// });

// // ✅ CORS - Allow Angular frontend + DevTunnels + Production
// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AllowFrontend",
//         policy =>
//         {
//             policy
//                 .AllowAnyHeader()
//                 .AllowAnyMethod()
//                 .AllowCredentials()
//                 .SetIsOriginAllowed(origin =>
//                 {
//                     if (origin == "http://localhost:4200")
//                         return true;

//                     if (origin == "https://fintcs.kritatechnosolutions.com")
//                         return true;

//                     try
//                     {
//                         var host = new Uri(origin).Host;
//                         return host.EndsWith("devtunnels.ms");
//                     }
//                     catch
//                     {
//                         return false;
//                     }
//                 });
//         });
// });

// // Register services
// builder.Services.AddScoped<UserService>();
// builder.Services.AddScoped<LoanTakenService>();

// var app = builder.Build();

// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// // ✅ Apply CORS before authentication/authorization
// app.UseCors("AllowFrontend");

// app.UseAuthentication();
// app.UseAuthorization();

// // Redirect root to Swagger
// app.MapGet("/", () => Results.Redirect("/swagger"));

// app.MapControllers();

// // Ensure database is created / migrated
// using (var scope = app.Services.CreateScope())
// {
//     var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//     context.Database.Migrate();
// }

// // ✅ Bind to all interfaces (for devtunnels & Angular)
// app.Run("http://0.0.0.0:5000");
