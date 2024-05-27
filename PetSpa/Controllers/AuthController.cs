using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PetSpa.Models.DTO.LoginDTO;
using PetSpa.Models.DTO.RegisterDTO;
using PetSpa.Repositories.Token;

namespace PetSpa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITokenRepository tokenRepository;
        public AuthController (UserManager<IdentityUser> userManager, ITokenRepository tokenRepository)
        {
            this._userManager = userManager;
            this.tokenRepository = tokenRepository;
        }
        //Post : /api/Auth/Register
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterPequestDto registerRequestDto)
        {
            var identityUser = new IdentityUser
            {
                UserName = registerRequestDto.Username,
                Email = registerRequestDto.Username
            };
         var identityResult =    await _userManager.CreateAsync(identityUser, registerRequestDto.Password);
            if(identityResult.Succeeded)
            {
                //Add roles to this User
                if(registerRequestDto.Roles != null  && registerRequestDto.Roles.Any()) {
                    identityResult =    await _userManager.AddToRolesAsync(identityUser, registerRequestDto.Roles);
                    if(identityResult.Succeeded )
                    {
                        return Ok("User was registered? Please login");
                    }
                }
            }
            return BadRequest("Something went wrong");
        }

        //Post: /api/Auth/Login
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var user = await _userManager.FindByEmailAsync(loginRequestDto.Username);
            if(user != null) {
                var checkPassworkResult = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
                if (checkPassworkResult)
                {
                    //Get Roles for this user
                    var roles = await _userManager.GetRolesAsync(user);
                    if(roles != null && roles.Any())
                    {
                        //Create Token
    var jwtToken =                     tokenRepository.CreateJWTToken(user, roles.ToList());
                        var response = new LoginResponseDto
                        {
                            JwtToken = jwtToken
                        };
                        return Ok(response);
                    }
                    
                    
                }
            }
            return BadRequest("Username or password incorrect");
        }
    }
}
