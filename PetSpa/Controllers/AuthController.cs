using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using PetSpa.CustomActionFilter;
using PetSpa.Data;
using PetSpa.Models.Domain;
using PetSpa.Models.DTO.ApiResponseDTO;
using PetSpa.Models.DTO.ForgotPasswoDDTO;
using PetSpa.Models.DTO.LoginDTO;
using PetSpa.Models.DTO.Pet;
using PetSpa.Models.DTO.RegisterDTO;
using PetSpa.Repositories.SendingEmail;
using PetSpa.Repositories.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetSpa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenRepository tokenRepository;
        private readonly PetSpaContext petSpaContext;
        private readonly IEmailSender _emailSender;
        private readonly ApiResponseService _apiResponse;

        public AuthController(UserManager<ApplicationUser> userManager, ITokenRepository tokenRepository, PetSpaContext petSpaContext, IEmailSender _emailSender, ApiResponseService apiResponse)
        {
            this._userManager = userManager;
            this.tokenRepository = tokenRepository;
            this.petSpaContext = petSpaContext;
            this._emailSender = _emailSender;
            this._apiResponse = apiResponse;
        }

        //Post : /api/Auth/Register
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterPequestDto registerRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var applicationUser = new ApplicationUser
            {
                UserName = registerRequestDto.Email,
                Email = registerRequestDto.Email
            };

            var identityResult = await _userManager.CreateAsync(applicationUser, registerRequestDto.Password);
            if (identityResult.Succeeded)
            {
                // Thêm vai trò cho người dùng này
                identityResult = await _userManager.AddToRolesAsync(applicationUser, new List<string> { "Customer" });
                if (identityResult.Succeeded)
                {
                    var customer = new Customer
                    {
                        CusId = Guid.NewGuid(),
                        FullName = registerRequestDto.FullName,
                        Gender = registerRequestDto.Gender,
                        PhoneNumber = registerRequestDto.PhoneNumber,
                        CusRank = "bronze",
                        Id = applicationUser.Id, // Liên kết với ApplicationUser
                    };

                    petSpaContext.Customers.Add(customer);
                    if (await petSpaContext.SaveChangesAsync() > 0)
                    {
                        return Ok("Người dùng đã được đăng ký. Vui lòng đăng nhập.");
                    }
                }
            }

            var errors = identityResult.Errors.Select(e => e.Description).ToList();
            return BadRequest(new { message = "Có gì đó sai sai", errors });
        }

        //Post: /api/Auth/Login
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var user = await _userManager.FindByEmailAsync(loginRequestDto.Username);
            if (user != null)
            {
                var checkPasswordResult = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);

                if (checkPasswordResult)
                {
                    // Lấy vai trò cho người dùng này
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles != null && roles.Any())
                    {
                        // Tạo Token
                        var data = new PetSpa.Models.DTO.ApiResponseDTO.Data
                        {
                            User = new User
                            {
                                Id = user.Id, // Không cần chuyển đổi nếu Id là Guid
                                Email = user.Email,
                                Name = user.UserName,
                            },
                            Token = tokenRepository.CreateJWTToken(user, roles[0], 15)
                        };

                        var response = _apiResponse.LoginSuccessResponse(data, "Đăng nhập thành công");
                        return Ok(response);
                    }
                }
            }
            return BadRequest("Tên người dùng hoặc mật khẩu không đúng");
        }

        [HttpPost("forgot-password")]
        //[Route("{email}")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest Fotgotpassword)
        {
            var user = await _userManager.FindByEmailAsync(Fotgotpassword.Email);

            if (user == null)
            {
                return BadRequest("User not found");
            }
            var roles = await _userManager.GetRolesAsync(user);
            if (roles != null && roles.Any())
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = $"http://localhost:5173/reset-password?token={token}&email={user.Email}"; // Đường link frontend

                // **Sử dụng HTML trong email**
                var emailBody = $@" Xin chào {user.UserName}
                   Reset Password,link here:{callbackUrl} ";

                var message = new Message(new string[] { user.Email }, "Đặt lại mật khẩu", emailBody);
                _emailSender.SendEmail(message);
                var response = _apiResponse.CreateSuccessResponse("Send email successfully");
                return Ok(token);

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

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ForgotPasswordRequestDto model)
        {


            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Không tiết lộ rằng người dùng không tồn tại
                return BadRequest("User does not find");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                var response = _apiResponse.CreateSuccessResponse("Reset password successfully");
                return Ok(response);
            }
            else
            {
                return BadRequest("Test");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return BadRequest("Wrong resetPassword");
        }
    }
}
