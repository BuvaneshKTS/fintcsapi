// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.EntityFrameworkCore;
// using FintcsApi.Data;
// using FintcsApi.Models;
// using FintcsApi.Services;
// using System.Security.Claims;

// namespace FintcsApi.Controllers
// {
//     [ApiController]
//     [Route("api/[controller]")]
//     [Authorize]
//     public class UsersController : ControllerBase
//     {
//         private readonly AppDbContext _context;
//         private readonly UserService _userService;

//         public UsersController(AppDbContext context, UserService userService)
//         {
//             _context = context;
//             _userService = userService;
//         }

//         // ----------- GET ALL USERS (Admin only) -----------
//         [HttpGet]
//         [Authorize(Roles = "admin")]
//         public async Task<IActionResult> GetUsers()
//         {
//             try
//             {
//                 var users = await _context.Users.ToListAsync();
//                 var userResponses = users.Select(u => {
//                     var userDetails = System.Text.Json.JsonSerializer.Deserialize<UserDetails>(u.Details) ?? new UserDetails();
//                     return new UserResponseDto
//                     {
//                         Id = u.Id,
//                         Username = u.Username,
//                         Email = u.Email,
//                         Phone = u.Phone,
//                         Roles = u.Role,
//                         Details = new UserDetailsDto
//                         {
//                             EDPNo = userDetails.EDPNo,
//                             Name = userDetails.Name,
//                             AddressOffice = userDetails.AddressOffice,
//                             AddressResidential = userDetails.AddressResidential,
//                             Designation = userDetails.Designation,
//                             PhoneOffice = userDetails.PhoneOffice,
//                             PhoneResidential = userDetails.PhoneResidential,
//                             Mobile = userDetails.Mobile,
//                             Email = userDetails.email
//                         },
//                         CreatedAt = u.CreatedAt
//                     };
//                 }).ToList();

//                 return Ok(new ApiResponse<List<UserResponseDto>>
//                 {
//                     Success = true,
//                     Message = "Users retrieved successfully",
//                     Data = userResponses
//                 });
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(500, new ApiResponse<object>
//                 {
//                     Success = false,
//                     Message = "Error retrieving users",
//                     Errors = new[] { ex.Message }
//                 });
//             }
//         }

//         // ----------- GET CURRENT USER -----------
//         [HttpGet("me")]
//         public async Task<IActionResult> GetCurrentUser()
//         {
//             try
//             {
//                 var userId = User.FindFirst("UserId")?.Value;
//                 if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int userIdInt))
//                 {
//                     return Unauthorized(new ApiResponse<object>
//                     {
//                         Success = false,
//                         Message = "Invalid user token"
//                     });
//                 }

//                 var user = await _context.Users.FindAsync(userIdInt);
//                 if (user == null)
//                 {
//                     return NotFound(new ApiResponse<object>
//                     {
//                         Success = false,
//                         Message = "User not found"
//                     });
//                 }

//                 var userDetails = System.Text.Json.JsonSerializer.Deserialize<UserDetails>(user.Details) ?? new UserDetails();
//                 var userResponse = new UserResponseDto
//                 {
//                     Id = user.Id,
//                     Username = user.Username,
//                     Email = user.Email,
//                     Phone = user.Phone,
//                     Roles = user.Role,
//                     Details = new UserDetailsDto
//                     {
//                         EDPNo = userDetails.EDPNo,
//                         Name = userDetails.Name,
//                         AddressOffice = userDetails.AddressOffice,
//                         AddressResidential = userDetails.AddressResidential,
//                         Designation = userDetails.Designation,
//                         PhoneOffice = userDetails.PhoneOffice,
//                         PhoneResidential = userDetails.PhoneResidential,
//                         Mobile = userDetails.Mobile,
//                         Email = userDetails.email
//                     },
//                     CreatedAt = user.CreatedAt
//                 };

//                 return Ok(new ApiResponse<UserResponseDto>
//                 {
//                     Success = true,
//                     Message = "User profile retrieved successfully",
//                     Data = userResponse
//                 });
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(500, new ApiResponse<object>
//                 {
//                     Success = false,
//                     Message = "Error retrieving user profile",
//                     Errors = new[] { ex.Message }
//                 });
//             }
//         }

//         // ----------- UPDATE USER ROLE (Admin only) -----------
//         [HttpPut("{id}/role")]
//         [Authorize(Roles = "admin")]
//         public async Task<IActionResult> UpdateUserRole(int id, [FromBody] string role)
//         {
//             try
//             {
//                 var user = await _context.Users.FindAsync(id);
//                 if (user == null)
//                 {
//                     return NotFound(new ApiResponse<object>
//                     {
//                         Success = false,
//                         Message = "User not found"
//                     });
//                 }

//                 // Validate role
//                 var validRoles = new[] { "user", "admin" };
//                 var validatedRole = validRoles.Contains(role?.ToLower()) ? role!.ToLower() : "user";

//                 // Update user details
//                 var currentDetails = System.Text.Json.JsonSerializer.Deserialize<UserDetails>(user.Details) ?? new UserDetails();
//                 currentDetails.role = validatedRole;
//                 user.Details = System.Text.Json.JsonSerializer.Serialize(currentDetails);

//                 await _context.SaveChangesAsync();

//                 var userResponse = new UserResponseDto
//                 {
//                     Id = user.Id,
//                     Username = user.Username,
//                     Email = user.Email,
//                     Phone = user.Phone,
//                     Roles = user.Role,
//                     Details = new UserDetailsDto
//                     {
//                         EDPNo = currentDetails.EDPNo,
//                         Name = currentDetails.Name,
//                         AddressOffice = currentDetails.AddressOffice,
//                         AddressResidential = currentDetails.AddressResidential,
//                         Designation = currentDetails.Designation,
//                         PhoneOffice = currentDetails.PhoneOffice,
//                         PhoneResidential = currentDetails.PhoneResidential,
//                         Mobile = currentDetails.Mobile,
//                         Email = currentDetails.email
//                     },
//                     CreatedAt = user.CreatedAt
//                 };

//                 return Ok(new ApiResponse<UserResponseDto>
//                 {
//                     Success = true,
//                     Message = "User role updated successfully",
//                     Data = userResponse
//                 });
//             }
//             catch (Exception ex)
//             {
//                 return StatusCode(500, new ApiResponse<object>
//                 {
//                     Success = false,
//                     Message = "Error updating user role",
//                     Errors = new[] { ex.Message }
//                 });
//             }
//         }
//     }
// }


using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using FintcsApi.Data;
using FintcsApi.Models;
using FintcsApi.Services;

namespace FintcsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserService _userService;

        public UsersController(AppDbContext context, UserService userService)
        {
            _context = context;
            _userService = userService;
        }

        // ------------------ GET ALL USERS (Admin) ------------------
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            var response = users.Select(ToUserResponseDto).ToList();

            return Ok(new ApiResponse<List<UserResponseDto>>
            {
                Success = true,
                Message = "Users retrieved successfully",
                Data = response
            });
        }

        // ------------------ GET USER BY ID (Admin) ------------------
        [HttpGet("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "User not found"
                });
            }

            return Ok(new ApiResponse<UserResponseDto>
            {
                Success = true,
                Message = "User retrieved successfully",
                Data = ToUserResponseDto(user)
            });
        }

        // ------------------ GET CURRENT USER ------------------
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int userIdInt))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid user token"
                });
            }

            var user = await _context.Users.FindAsync(userIdInt);
            if (user == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "User not found"
                });
            }

            return Ok(new ApiResponse<UserResponseDto>
            {
                Success = true,
                Message = "User profile retrieved successfully",
                Data = ToUserResponseDto(user)
            });
        }

        // ------------------ CREATE USER (Admin) ------------------
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            var newUser = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                Phone = dto.Phone,
                Role = dto.Role ?? "user",
                Details = System.Text.Json.JsonSerializer.Serialize(dto.Details),
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<UserResponseDto>
            {
                Success = true,
                Message = "User created successfully",
                Data = ToUserResponseDto(newUser)
            });
        }

        // ------------------ UPDATE USER (Admin) ------------------
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "User not found"
                });
            }

            user.Username = dto.Username ?? user.Username;
            user.Email = dto.Email ?? user.Email;
            user.Phone = dto.Phone ?? user.Phone;
            user.Role = dto.Role ?? user.Role;
            user.Details = System.Text.Json.JsonSerializer.Serialize(dto.Details);

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<UserResponseDto>
            {
                Success = true,
                Message = "User updated successfully",
                Data = ToUserResponseDto(user)
            });
        }

        // ------------------ UPDATE USER ROLE (Admin) ------------------
        [HttpPut("{id}/role")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] string role)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "User not found"
                });
            }

            var validRoles = new[] { "user", "admin" };
            user.Role = validRoles.Contains(role?.ToLower()) ? role.ToLower() : "user";

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<UserResponseDto>
            {
                Success = true,
                Message = "User role updated successfully",
                Data = ToUserResponseDto(user)
            });
        }

        // ------------------ DELETE USER (Admin) ------------------
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "User not found"
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

        // ------------------ Helper ------------------
        private UserResponseDto ToUserResponseDto(User u)
        {
            var userDetails = System.Text.Json.JsonSerializer.Deserialize<UserDetails>(u.Details) ?? new UserDetails();
            return new UserResponseDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                Phone = u.Phone,
                Roles = u.Role,
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
                CreatedAt = u.CreatedAt
            };
        }
    }
}
