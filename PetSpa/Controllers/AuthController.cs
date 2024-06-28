using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using PetSpa.CustomActionFilter;
using PetSpa.Data;
using PetSpa.Models.Domain;
using PetSpa.Models.DTO.ApiResponseDTO;
using PetSpa.Models.DTO.ChangePasswordRequestDto;
using PetSpa.Models.DTO.ForgotPasswoDDTO;
using PetSpa.Models.DTO.LoginDTO;
using PetSpa.Models.DTO.Pet;
using PetSpa.Models.DTO.RegisterDTO;
using PetSpa.Repositories.SendingEmail;
using PetSpa.Repositories.Token;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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



        private static string GenerateRandomPassword(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [HttpPost("google")]
        [Authorize(Roles = "Customer")]

        public async Task<IActionResult> CreateUserGoogle([FromBody] LoginGG email)
        {
            try
            {
                if (string.IsNullOrEmpty(email.Email))
                {
                    return BadRequest(_apiResponse.CreateErrorResponse("Email is required"));
                }

                var user = await _userManager.FindByEmailAsync(email.Email);

                if (user != null)
                {
                    var cus = await petSpaContext.Customers.FirstOrDefaultAsync(x => x.Id == user.Id);
                    var data = new PetSpa.Models.DTO.ApiResponseDTO.Data
                    {
                        User = new User
                        {
                            Id = cus?.CusId ?? user.Id,
                            Email = user.Email,
                            Name = user.Email,
                        },
                        Token = tokenRepository.CreateJWTToken(user, "Customer", 15)
                    };
                    return Ok(_apiResponse.LoginSuccessResponse(data, "Login Succeeded"));
                }
                else
                {
                    var newUser = new ApplicationUser
                    {
                        UserName = email.Email,
                        Email = email.Email
                    };

                    // Generate a random password for the new user
                    var randomPassword = GenerateRandomPassword(8)+"@A2a";
                    var createResult = await _userManager.CreateAsync(newUser, randomPassword);

                    if (createResult.Succeeded)
                    {
                        var roleResult = await _userManager.AddToRoleAsync(newUser, "Customer");
                        if (roleResult.Succeeded)
                        {
                            var createdUser = await _userManager.FindByEmailAsync(email.Email);
                            if (createdUser != null)
                            {
                                var newCus = new Customer
                                {
                                    Id = createdUser.Id,
                                    CusRank = "bronze"
                                   
                                    // Set other properties as necessary
                                };
                                await petSpaContext.Customers.AddAsync(newCus);
                                await petSpaContext.SaveChangesAsync();

                                var savedCus = await petSpaContext.Customers.FirstOrDefaultAsync(x => x.Id == createdUser.Id);
                                if (savedCus == null)
                                {
                                    return BadRequest(_apiResponse.CreateErrorResponse("Error retrieving saved customer"));
                                }
                                var callbackUrl = $"{randomPassword}"; // Đường link frontend

                                // **Sử dụng HTML trong email**
                                var emailBody = $@" Xin chào {newUser.Email}
                             Password,link here:{callbackUrl} ";

                                var message = new Message(new string[] { createdUser.Email }, "Password", emailBody);
                                _emailSender.SendEmail(message);
                                var response = _apiResponse.CreateSuccessResponse("Send email successfully");

                                var data = new PetSpa.Models.DTO.ApiResponseDTO.Data
                                {
                                    User = new User
                                    {
                                        Id = savedCus.CusId,
                                        Email = createdUser.Email,
                                        Name = createdUser.UserName,
                                    },
                                    Token = tokenRepository.CreateJWTToken(createdUser, "Customer", 15)
                                };


                                // **Sử dụng HTML trong email**
                                
                                return Ok(_apiResponse.LoginSuccessResponse(data, "Register Succeeded"));
                            }
                            else
                            {
                                return BadRequest(_apiResponse.CreateErrorResponse("Error finding user after creation"));
                            }
                        }
                        else
                        {
                            return BadRequest(_apiResponse.CreateErrorResponse("Error adding role to user"));
                        }
                    }
                    else
                    {
                        return BadRequest(_apiResponse.CreateErrorResponse("Error creating user"));
                    }
                }
            }
            catch (Exception ex)
            {
               
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
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

            var userResult = await _userManager.FindByEmailAsync(registerRequestDto.Email);
            if (userResult == null)
            {
                // Kiểm tra số điện thoại đã được đăng ký chưa
                var userByPhoneNumber = await _userManager.FindByNameAsync(registerRequestDto.PhoneNumber);
                if (userByPhoneNumber != null)
                {
                    return BadRequest(_apiResponse.CreateErrorResponse("Phone number is already registered"));
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
                        var user = await _userManager.FindByEmailAsync(registerRequestDto.Email);
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
                            var data = new PetSpa.Models.DTO.ApiResponseDTO.Data
                            {
                                User = new User
                                {
                                    Id = customer.CusId,
                                    Email = user.Email,
                                    Name = user.UserName,
                                },
                                Token = tokenRepository.CreateJWTToken(user, "Customer", 15)
                            };

                            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                            var callbackUrl = $"http://localhost:5173/comfirmed-email?token={token}&email={user.Email}"; // Đường link frontend

                            // **Sử dụng HTML trong email**
                            var emailBody = $@" Hello {user.UserName}
           VerifyEmail,link here:{callbackUrl} ";

                            var message = new Message(new string[] { user.Email }, "Verify", emailBody);
                            _emailSender.SendEmail(message);

                            var response = _apiResponse.LoginSuccessResponse(data, "Register Successed");
                            return Ok(response);
                        }
                    }

                    var errors = identityResult.Errors.Select(e => e.Description).ToList();
                    return BadRequest(_apiResponse.CreateErrorResponse("Error register"));
                }
            }

            return BadRequest(_apiResponse.CreateErrorResponse("Email is already registered"));

        }
        [HttpGet("ConfirmEmail")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ComfirmEmailDTO request)
        {
            if (string.IsNullOrEmpty(request.Token) || string.IsNullOrEmpty(request.Email))
            {
                return BadRequest(new { message = "Invalid email confirmation request." });
            }

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return BadRequest(new { message = "Invalid email confirmation request." });
            }

            var result = await _userManager.ConfirmEmailAsync(user, request.Token);
            if (result.Succeeded)
            {
                return Ok(new { message = "Email confirmed successfully." });
            }

            return BadRequest(new { message = "Error confirming your email." });
        }

        //Post: /api/Auth/Login
        [HttpPost]
        [Route("Login")]
       

        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var user = await _userManager.FindByEmailAsync(loginRequestDto.Username);
            if (user != null)
            {
                bool hasPassword = await _userManager.HasPasswordAsync(user);
                if (!hasPassword)
                {
                    return BadRequest(_apiResponse.CreateErrorResponse("Tài khoản đã được đăng ký bằng Google. Vui lòng đăng nhập bằng Google."));
                }
                var checkPasswordResult = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);

                if (checkPasswordResult)
                {
                    // Lấy vai trò cho người dùng này
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles != null && roles.Any())
                    {
                        // Tạo Token
                        var cus = await petSpaContext.Customers.FirstOrDefaultAsync(x => x.Id == user.Id);
                        var data = new PetSpa.Models.DTO.ApiResponseDTO.Data
                        {
                            User = new User
                            {
                                Id = cus.CusId, // Không cần chuyển đổi nếu Id là Guid
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
        [HttpPost]
        [Route("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto changePasswordRequestDto)
        {
            var user = await _userManager.FindByEmailAsync(changePasswordRequestDto.Email);
            if (user != null)
            {
                bool hasPassword = await _userManager.HasPasswordAsync(user);
                if (!hasPassword)
                {
                    // User is registered via external provider, cannot change password
                    return BadRequest(_apiResponse.CreateErrorResponse("This account is registered via external provider. Please use the appropriate method to change the password."));
                }

                // Check if the current password is correct
                var checkPasswordResult = await _userManager.CheckPasswordAsync(user, changePasswordRequestDto.CurrentPassword);
                if (!checkPasswordResult)
                {
                    // Current password is incorrect
                    return BadRequest(_apiResponse.CreateErrorResponse("Incorrect current password."));
                }

                // Change the password
                var changePasswordResult = await _userManager.ChangePasswordAsync(user, changePasswordRequestDto.CurrentPassword, changePasswordRequestDto.NewPassword);
                if (changePasswordResult.Succeeded)
                {
                    return Ok(_apiResponse.CreateSuccessResponse("Password changed successfully."));
                }
                else
                {
                    // Failed to change password
                    return BadRequest(_apiResponse.CreateErrorResponse("Failed to change password."));
                }
            }
            return BadRequest(_apiResponse.CreateErrorResponse("User not found "));
        }

        [HttpPost("forgot-password")]
        [Authorize(Roles = "Admin,Customer,Staff,Manager")]
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
        [Authorize(Roles = "Admin,Customer,Staff,Manager")]

        public async Task<IActionResult> TestEmail()
        {
            var message = new Message(new string[] { "nguyenbaminhduc2019@gmail.com" }, "Test", "<h1>Hoàng Anh stupid</h1>");
            _emailSender.SendEmail(message);
            return Ok("Success");
        }

        [HttpPost("reset-password")]
        [Authorize(Roles = "Admin,Customer,Staff,Manager")]

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
