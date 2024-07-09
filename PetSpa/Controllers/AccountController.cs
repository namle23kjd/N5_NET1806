using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetSpa.Data;
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
        private readonly PetSpaContext context;
        private readonly PetSpaContext petSpaContext;

        public AccountController(UserManager<ApplicationUser> userManager, IMapper mapper, RoleManager<IdentityRole<Guid>> roleManager, PetSpaContext context, IUserRepository userRepository)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this._mapper = mapper;
            this._roleManager = roleManager;
            this.context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            // Lấy tất cả người dùng có Status là true
            var users = await _userManager.Users
                .Where(u => u.Status)
                .ToListAsync();

            // Lọc người dùng theo các vai trò mong muốn
            var filteredUsers = new List<ApplicationUser>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Staff") || roles.Contains("Customer") || roles.Contains("Manager"))
                {
                    filteredUsers.Add(user); // Thêm vào danh sách đã lọc
                }
            }

            var userDtos = _mapper.Map<List<UserDTO>>(filteredUsers);

            foreach (var userDto in userDtos)
            {
                var user = filteredUsers.First(u => u.UserName == userDto.UserName);
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
                PhoneNumber = registerUserDTO.PhoneNumber,
                Status = true // Khi tạo mới, đặt Status là true
            };

            var result = await _userManager.CreateAsync(user, registerUserDTO.Password);

            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(registerUserDTO.Role))
                {
                    await _roleManager.CreateAsync(new IdentityRole<Guid> { Name = registerUserDTO.Role });
                }

                await _userManager.AddToRoleAsync(user, registerUserDTO.Role);

                // Tạo đối tượng trong bảng tương ứng với vai trò
                switch (registerUserDTO.Role.ToLower())
                {
                    case "customer":
                        var customer = new Customer
                        {
                            CusId = Guid.NewGuid(),
                            Id = user.Id,
                            FullName = user.UserName,
                            PhoneNumber = registerUserDTO.PhoneNumber,
                            CusRank = "Bronze", // Đặt hạng mặc định là Bronze
                            TotalSpent = 0,
                            Gender = registerUserDTO.Gender,
                        };
                        context.Customers.Add(customer);
                        break;

                    case "staff":
                        var staff = new Staff
                        {
                            StaffId = Guid.NewGuid(),
                            Id = user.Id,
                            FullName = registerUserDTO.FullName,
                            Gender = registerUserDTO.Gender,
                        };
                        context.Staff.Add(staff);
                        break;

                    case "admin":
                        var admin = new Admin
                        {
                            AdminId = Guid.NewGuid(),
                            Id = user.Id
                        };
                        context.Admins.Add(admin);
                        break;

                    case "manager":
                        var manager = new Manager
                        {
                            ManaId = Guid.NewGuid(),
                            Id = user.Id,
                            PhoneNumber = registerUserDTO.PhoneNumber,
                            FullName = registerUserDTO.FullName,
                            Gender = registerUserDTO.Gender,
                        };
                        context.Managers.Add(manager);
                        break;
                }

                await context.SaveChangesAsync();

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
            var activeUsersInRole = usersInRole.Where(u => u.Status).ToList(); // Lọc người dùng có Status là true

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
            if (user == null || !user.Status)
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

        // Thay đổi trạng thái người dùng
        [HttpPost("change-status")]
        public async Task<IActionResult> ChangeUserStatus(Guid userId, bool status)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            user.Status = status;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(new { message = "User status updated successfully!" });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return BadRequest(ModelState);
        }
        // Xóa tài khoản (chuyển trạng thái thành false)
        [HttpDelete("delete-account/{userId}")]
        public async Task<IActionResult> DeleteAccount(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            user.Status = false;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(new { message = "User account deleted successfully!" });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return BadRequest(ModelState);
        }

        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUser (UpdateUserDTO updateUserDTO)
        {
            // Kiểm tra sự tồn tại của email
            var existingUserWithEmail = await _userManager.Users
                .Where(u => u.Email == updateUserDTO.Email && u.Id != updateUserDTO.Id)
                .FirstOrDefaultAsync();
            if (existingUserWithEmail != null)
            {
                return BadRequest("Email already exists.");
            }

            // Kiểm tra sự tồn tại của tên người dùng
            var existingUserWithUserName = await _userManager.Users
                .Where(u => u.UserName == updateUserDTO.UserName && u.Id != updateUserDTO.Id)
                .FirstOrDefaultAsync();
            if (existingUserWithUserName != null)
            {
                return BadRequest("Username already exists.");
            }

            // Kiểm tra sự tồn tại của số điện thoại
            var existingUserWithPhoneNumber = await _userManager.Users
                .Where(u => u.PhoneNumber == updateUserDTO.PhoneNumber && u.Id != updateUserDTO.Id)
                .FirstOrDefaultAsync();
            if (existingUserWithPhoneNumber != null)
            {
                return BadRequest("Phone number already exists.");
            }

            var user = await _userManager.FindByIdAsync(updateUserDTO.Id.ToString());
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.UserName = updateUserDTO.UserName;
            user.Email = updateUserDTO.Email;
            user.PhoneNumber = updateUserDTO.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // Cập nhật vai trò người dùng
            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                return BadRequest("Failed to remove current roles.");
            }

            if (!await _roleManager.RoleExistsAsync(updateUserDTO.Role))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid> { Name = updateUserDTO.Role });
            }

            var addResult = await _userManager.AddToRoleAsync(user, updateUserDTO.Role);
            if (!addResult.Succeeded)
            {
                return BadRequest("Failed to add new role.");
            }

            return Ok("User updated successfully.");
        }
    }
}
