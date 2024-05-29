using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetSpa.Data;
using PetSpa.Helper;
using PetSpa.Models.Domain;
using PetSpa.Models.DTO.ForgotPasswoDDTO;
using PetSpa.Models.DTO.LoginDTO;
using PetSpa.Models.DTO.RegisterDTO;
using PetSpa.Repositories.SendingEmail;
using PetSpa.Repositories.Token;
using System.Data;

namespace PetSpa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITokenRepository tokenRepository;
        private readonly PetSpaContext petSpaContext;
        private readonly IEmailSender _emailSender;
        public AuthController(UserManager<IdentityUser> userManager, ITokenRepository tokenRepository, PetSpaContext petSpaContext, IEmailSender _emailSender)
        {
            this._userManager = userManager;
            this.tokenRepository = tokenRepository;
            this.petSpaContext = petSpaContext;
            this._emailSender = _emailSender;
        }
        //Post : /api/Auth/Register
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterPequestDto registerRequestDto)
        {
            var identityUser = new IdentityUser
            {
                UserName = registerRequestDto.Email,
                Email = registerRequestDto.Email
            };
            var identityResult = await _userManager.CreateAsync(identityUser, registerRequestDto.Password);
            if (identityResult.Succeeded)
            {
                //Add roles to this User
                
                identityResult = await _userManager.AddToRolesAsync(identityUser, ["Customer"]);
                    if (identityResult.Succeeded)
                    {

                        //Add Account new
                        var user = await _userManager.FindByEmailAsync(registerRequestDto.Email);
                   
                    var account = new Account()
                        {
                            AccId = new Guid(),
                            Role = "Customer",
                            UserName = user.Id,
                            PassWord = user.PasswordHash,
                        };
                    var customer = new Customer()
                    {
                        CusId = Guid.NewGuid(),
                        FullName = registerRequestDto.FullName,
                        Gender = registerRequestDto.Gender,
                        PhoneNumber = registerRequestDto.PhoneNumber,
                        CusRank = registerRequestDto.CusRank,
                        AccId = account.AccId
                    };


                        petSpaContext.Accounts.Add(account);
                        petSpaContext.Customers.Add(customer);
                        if (await petSpaContext.SaveChangesAsync() > 0)  return Ok("User was registered? Please login");
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
            if (user != null)
            {
                var checkPassworkResult = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
              
                if (checkPassworkResult)
                {
                    //Get Roles for this user
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles != null && roles.Any())
                    {
                        //Create Token
                        var jwtToken = tokenRepository.CreateJWTToken(user, roles[0], 15);
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
        
        [HttpPost("forgot-password")]
        //[Route("{email}")]
        public async Task<IActionResult> ForgotPassword([FromBody]string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            
            if (user == null)
            {
                return BadRequest("User not found");
            }
            var roles = await _userManager.GetRolesAsync(user);
            if (roles != null && roles.Any() )
            {

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, token = token }, protocol: Request.Scheme);

                var message = new Message(new string[] { email }, "ResPassword", "Please reset your password by clicking <a>" + callbackUrl + token + "</a>");
                 _emailSender.SendEmail(message);
                
                    return Ok("SSuccess");
                
               

                   
                //if (await petSpaContext.SaveChangesAsync() > 0) return Ok("User was registered? Please login");
                //return BadRequest("Wrong");


            }
            return BadRequest("Email does not exist incorrect");
        }
        [HttpGet]
        public async Task<IActionResult> TestEmail()
        {
            var message = new Message(new string[] { "nguyenbaminhduc2019@gmail.com" }, "Test", "<h1>Hoàng Anh stupid</h1>");
            _emailSender.SendEmail(message);
            return Ok("Success");
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromBody]ForgotPasswordRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Không tiết lộ rằng người dùng không tồn tại
                return BadRequest("User does not find");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                return Ok("Reset password Successfully");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return BadRequest("Wrong resetPassword") ;
        }
    }
}
