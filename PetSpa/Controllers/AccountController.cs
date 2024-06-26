using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetSpa.Models.Domain;
using PetSpa.Models.DTO.UserDTO;
using PetSpa.Repositories.UsersRepository;

namespace PetSpa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public AccountController(UserManager<ApplicationUser> userManager, IMapper mapper, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this._mapper = mapper;
            this._roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = _mapper.Map<List<UserDTO>>(users);

            foreach (var userDto in userDtos)
            {
                var user = users.First(u => u.UserName == userDto.UserName);
                var roles = await _userManager.GetRolesAsync(user);
                userDto.Roles = roles.ToList();
            }

            return Ok(userDtos);

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDTO registerUserDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new ApplicationUser
            {
                UserName = registerUserDTO.UserName,
                Email = registerUserDTO.Email,
                PhoneNumber = registerUserDTO.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, registerUserDTO.Password);

            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(registerUserDTO.Role))
                {
                    await _roleManager.CreateAsync(new IdentityRole<Guid> { Name = registerUserDTO.Role });
                }

                await _userManager.AddToRoleAsync(user, registerUserDTO.Role);

                return Ok(new { message = "User created successfully!" });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return BadRequest(ModelState);
        }
        [HttpPost("change-role")]
        public async Task<IActionResult> ChangeUserRole(ChangUserRoleDTO changeUserRoleDTO)
        {
            var user = await _userManager.FindByIdAsync(changeUserRoleDTO.Id.ToString());
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!removeResult.Succeeded)
            {
                return BadRequest(new { message = "Failed to remove current roles" });
            }

            if (!await _roleManager.RoleExistsAsync(changeUserRoleDTO.NewRole))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid> { Name = changeUserRoleDTO.NewRole });
            }

            var addResult = await _userManager.AddToRoleAsync(user, changeUserRoleDTO.NewRole);
            if (!addResult.Succeeded)
            {
                return BadRequest(new { message = "Failed to add new role" });
            }

            return Ok(new { message = "User role updated successfully!" });
        }
        // Tìm tài khoản theo vai trò
        [HttpGet("users-by-role")]
        public async Task<IActionResult> GetUsersByRole(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                return NotFound(new { message = "Role not found" });
            }

            var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);

            var userDTOs = usersInRole.Select(user => new UserRoleDTO
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Roles = new List<string> { roleName }
            }).ToList();

            return Ok(userDTOs);
        }

        // Tìm vai trò theo tài khoản
        [HttpGet("roles-by-user/{userId}")]
        public async Task<IActionResult> GetRolesByUser(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var roles = await _userManager.GetRolesAsync(user);
            var userRoleDTO = new UserRoleDTO
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Roles = roles.ToList()
            };

            return Ok(userRoleDTO);
        }
    }
}
