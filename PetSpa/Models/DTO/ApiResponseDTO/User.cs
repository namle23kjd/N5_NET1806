﻿namespace PetSpa.Models.DTO.ApiResponseDTO
{
    public class User
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
    }
}
