using Domain.Entity;
using Microsoft.AspNetCore.Mvc;
using TicketingSystem.Services;
using Travelsite.DTOs;

namespace TicketingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Get all users
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                var userDtos = users.Select(MapToUserDto);
                if (!userDtos.Any())
                    return NotFound(new { message = "No users found" });
                // Use ApiResponse to return the list of users
                return Ok(new ApiResponse<IEnumerable<UserDto>>(200, userDtos, "Users retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving users", error = ex.Message });
            }
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound(new { message = "User not found" });
                // Use ApiResponse to return the user details
                var userDto = MapToUserDto(user);
                return Ok(new ApiResponse<UserDto>(200, userDto, "User retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the user", error = ex.Message });
            }
        }

        /// <summary>
        /// Create new user
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = new User
                {
                    FullName = createUserDto.FullName,
                    Email = createUserDto.Email,
                    Phone = createUserDto.Phone,
                    Category = createUserDto.Type
                };

                var createdUser = await _userService.CreateUserAsync(user);
                var userDto = MapToUserDto(createdUser);
                // Use ApiResponse to return the created user details
                if (userDto == null)
                    return BadRequest(new { message = "User could not be created" });
                return Ok(new ApiResponse<UserDto>(201, userDto, "User created successfully"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the user", error = ex.Message });
            }
        }




        private static UserDto MapToUserDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Category = user.Category,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
