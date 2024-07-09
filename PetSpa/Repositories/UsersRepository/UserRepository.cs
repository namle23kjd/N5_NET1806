using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetSpa.Data;
using PetSpa.Models.Domain;
using PetSpa.Models.DTO.ApiResponseDTO;
using PetSpa.Models.DTO.UserDTO;

namespace PetSpa.Repositories.UsersRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public UserRepository(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<UserDTO>> GetAllUserAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            return _mapper.Map<List<UserDTO>>(users);
        }
    }
}