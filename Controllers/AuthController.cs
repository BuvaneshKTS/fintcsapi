using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using FintcsApi.Data;
using FintcsApi.Models;
using FintcsApi.Services;
using Microsoft.AspNetCore.Authorization;


namespace FintcsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly UserService _userService;

        // Valid roles in the system
        private readonly string[] ValidRoles = { "user", "admin" };

        public AuthController(AppDbContext context, IConfiguration config, UserService userService)
        {
            _context = context;
            _config = config;
            _userService = userService;
        }

        // ----------- REGISTER -----------
        [HttpPost("register")]
        // [Authorize(Roles = "admin")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                // Validate input
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid input data",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToArray()
                    });
                }

                // Check if user already exists
                if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User already exists",
                        Errors = new[] { $"Username '{dto.Username}' is already taken" }
                    });
                }

                // Check if email already exists
                var existingUserWithEmail = await _context.Users.FirstOrDefaultAsync(u => u.Details.Contains(dto.Email));
                if (existingUserWithEmail != null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Email already registered",
                        Errors = new[] { $"Email '{dto.Email}' is already registered" }
                    });
                }

                // Create user details object (default role is "user")
                var details = new UserDetails
                {
                    email = dto.Email,
                    phone = dto.Phone ?? string.Empty,
                    role = "admin", // Always default to user role
                    EDPNo = dto.EDPNo ?? string.Empty,
                    Name = dto.Name ?? string.Empty,
                    AddressOffice = dto.AddressOffice ?? string.Empty,
                    AddressResidential = dto.AddressResidential ?? string.Empty,
                    Designation = dto.Designation ?? string.Empty,
                    PhoneOffice = dto.PhoneOffice ?? string.Empty,
                    PhoneResidential = dto.PhoneResidential ?? string.Empty,
                    Mobile = dto.Mobile ?? string.Empty
                };

                // Create new user
                var user = new User
                {
                    Username = dto.Username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    Details = JsonSerializer.Serialize(details)
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var userResponse = new UserResponseDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Phone = user.Phone,
                    Roles = user.Role,
                    Details = new UserDetailsDto
                    {
                        EDPNo = dto.EDPNo ?? string.Empty,
                        Name = dto.Name ?? string.Empty,
                        AddressOffice = dto.AddressOffice ?? string.Empty,
                        AddressResidential = dto.AddressResidential ?? string.Empty,
                        Designation = dto.Designation ?? string.Empty,
                        PhoneOffice = dto.PhoneOffice ?? string.Empty,
                        PhoneResidential = dto.PhoneResidential ?? string.Empty,
                        Mobile = dto.Mobile ?? string.Empty,
                        Email = dto.Email
                    },
                    CreatedAt = user.CreatedAt
                };

                return Ok(new ApiResponse<UserResponseDto>
                {
                    Success = true,
                    Message = "User registered successfully",
                    Data = userResponse
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Internal server error occurred during registration",
                    Errors = new[] { ex.Message }
                });
            }
        }

        // ----------- LOGIN -----------
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                // Validate input
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid input data",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToArray()
                    });
                }

                // Find user by username
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
                
                // Verify user exists and password is correct
                if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid credentials",
                        Errors = new[] { "Username or password is incorrect" }
                    });
                }

                // Generate JWT token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? "your-super-secret-jwt-key-that-should-be-at-least-32-characters");
                var expiresAt = DateTime.UtcNow.AddHours(2);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim("UserId", user.Id.ToString()),
                    new Claim("Email", user.Email)
                };

                // Add role claim
                claims.Add(new Claim(ClaimTypes.Role, user.Role));

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = expiresAt,
                    Issuer = _config["Jwt:Issuer"],
                    Audience = _config["Jwt:Audience"],
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                var userDetails = System.Text.Json.JsonSerializer.Deserialize<UserDetails>(user.Details) ?? new UserDetails();
                var loginResponse = new LoginResponseDto
                {
                    Token = tokenString,
                    Username = user.Username,
                    Email = user.Email,
                    Phone = user.Phone,
                    Roles = user.Role,
                    Details = new UserDetailsDto
                    {
                        EDPNo = userDetails.EDPNo,
                        Name = userDetails.Name,
                        AddressOffice = userDetails.AddressOffice,
                        AddressResidential = userDetails.AddressResidential,
                        Designation = userDetails.Designation,
                        PhoneOffice = userDetails.PhoneOffice,
                        PhoneResidential = userDetails.PhoneResidential,
                        Mobile = userDetails.Mobile,
                        Email = userDetails.email
                    },
                    ExpiresAt = expiresAt
                };

                return Ok(new ApiResponse<LoginResponseDto>
                {
                    Success = true,
                    Message = "Login successful",
                    Data = loginResponse
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Internal server error occurred during login",
                    Errors = new[] { ex.Message }
                });
            }
        }


        // ----------- UPDATE USER -----------
        [HttpPut("update/{id}")]
        [Authorize(Roles = "admin,user")] // Adjust roles as needed
        public async Task<IActionResult> UpdateUser(int id, [FromBody] RegisterDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid input data",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToArray()
                    });
                }

                // Find user by Id
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User not found",
                        Errors = new[] { $"No user exists with Id {id}" }
                    });
                }

                // Check if username is taken by another user
                if (await _context.Users.AnyAsync(u => u.Username == dto.Username && u.Id != id))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Username already taken",
                        Errors = new[] { $"Username '{dto.Username}' is already in use" }
                    });
                }

                // Check if email is taken by another user
                var existingUserWithEmail = await _context.Users
                    .Where(u => u.Id != id)
                    .FirstOrDefaultAsync(u => u.Details.Contains(dto.Email));

                if (existingUserWithEmail != null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Email already registered",
                        Errors = new[] { $"Email '{dto.Email}' is already registered" }
                    });
                }

                // Update user details
                var details = new UserDetails
                {
                    email = dto.Email,
                    phone = dto.Phone ?? string.Empty,
                    role = "admin", // Or you can allow updating role from dto if needed
                    EDPNo = dto.EDPNo ?? string.Empty,
                    Name = dto.Name ?? string.Empty,
                    AddressOffice = dto.AddressOffice ?? string.Empty,
                    AddressResidential = dto.AddressResidential ?? string.Empty,
                    Designation = dto.Designation ?? string.Empty,
                    PhoneOffice = dto.PhoneOffice ?? string.Empty,
                    PhoneResidential = dto.PhoneResidential ?? string.Empty,
                    Mobile = dto.Mobile ?? string.Empty
                };

                user.Username = dto.Username;
                user.PasswordHash = string.IsNullOrEmpty(dto.Password) ? user.PasswordHash : BCrypt.Net.BCrypt.HashPassword(dto.Password);
                user.Details = JsonSerializer.Serialize(details);

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                // Prepare response
                var userResponse = new UserResponseDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Phone = user.Phone,
                    Roles = user.Role,
                    Details = new UserDetailsDto
                    {
                        EDPNo = dto.EDPNo ?? string.Empty,
                        Name = dto.Name ?? string.Empty,
                        AddressOffice = dto.AddressOffice ?? string.Empty,
                        AddressResidential = dto.AddressResidential ?? string.Empty,
                        Designation = dto.Designation ?? string.Empty,
                        PhoneOffice = dto.PhoneOffice ?? string.Empty,
                        PhoneResidential = dto.PhoneResidential ?? string.Empty,
                        Mobile = dto.Mobile ?? string.Empty,
                        Email = dto.Email
                    },
                    CreatedAt = user.CreatedAt
                };

                return Ok(new ApiResponse<UserResponseDto>
                {
                    Success = true,
                    Message = "User updated successfully",
                    Data = userResponse
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Internal server error occurred during update",
                    Errors = new[] { ex.Message }
                });
            }
        }


        // ----------- DELETE USER -----------
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "admin")] // Only admin can delete users
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                // Find user by Id
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User not found",
                        Errors = new[] { $"No user exists with Id {id}" }
                    });
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "User deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Internal server error occurred during delete",
                    Errors = new[] { ex.Message }
                });
            }
        }

        // ----------- GET VALID ROLES -----------
        [HttpGet("roles")]
        public IActionResult GetValidRoles()
        {
            return Ok(new ApiResponse<string[]>
            {
                Success = true,
                Message = "Valid roles retrieved successfully",
                Data = ValidRoles
            });
        }
    }
}
