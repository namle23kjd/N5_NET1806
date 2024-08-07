﻿namespace PetSpa.Models.DTO.UserDTO
{
    public class RegisterUserDTO
    {
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty ;
        public string Gender {  get; set; } = string.Empty ;    
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
