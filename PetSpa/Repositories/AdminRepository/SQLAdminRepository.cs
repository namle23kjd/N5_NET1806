using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetSpa.Data;
using PetSpa.Models.Domain;
using PetSpa.Models.DTO.Admin;

namespace PetSpa.Repositories.AdminRepository
{
    public class SQLAdminRepository : IAdminRepository
    {
        private readonly PetSpaContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<SQLAdminRepository> _logger;

        public SQLAdminRepository(PetSpaContext dbContext, UserManager<ApplicationUser> _userManager, ILogger<SQLAdminRepository> logger)
        {
            this._dbContext = dbContext;
            this._userManager = _userManager;
            this._logger = logger;
        }

        public async Task<bool> CreateAdminAsync(AddAdminRequestDTO adminRequest)
        {
            try
            {
                // Tạo một đối tượng ApplicationUser mới
                var user = new ApplicationUser
                {
                    UserName = adminRequest.UserName,
                    Email = adminRequest.Email,
                    EmailConfirmed = true
                };

                // Tạo người dùng mới trong bảng AspNetUsers
                var result = await _userManager.CreateAsync(user, adminRequest.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        _logger.LogError("Error creating user: Code={Code}, Description={Description}", error.Code, error.Description);
                    }
                    return false;
                }

                // Thêm người dùng vào role "Admin"
                await _userManager.AddToRoleAsync(user, "Admin");

                // Tạo bản ghi mới trong bảng Admin
                var admin = new Admin
                {
                    AdminId = Guid.NewGuid(),
                    Id = user.Id
                };

                _dbContext.Admins.Add(admin);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating admin.");
                return false;
            }
        }

        public async Task<List<Admin>> GetAllAsync()
        {
            try
            {
                return await _dbContext.Admins.Include(a => a.User).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all admins.");
                return new List<Admin>(); // Trả về danh sách rỗng trong trường hợp lỗi
            }
        }

        public async Task<Admin> UpdateAsync(Guid AdminId, Admin admin)
        {
            var existingAdmin = await _dbContext.Admins.FindAsync(AdminId);
            if (existingAdmin == null)
            {
                return null;
            }

            existingAdmin.Id = admin.Id;
            await _dbContext.SaveChangesAsync();

            return existingAdmin;
        }
        public async Task<Admin> FindAdminAsync(Guid AdminId)
        {
            try
            {
                return await _dbContext.Admins.Include(a => a.User)  // Bao gồm thông tin User
                                       .FirstOrDefaultAsync(a => a.AdminId == AdminId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while finding admin.");
                return null; // Trả về null trong trường hợp lỗi
            }
        }
    }
}
