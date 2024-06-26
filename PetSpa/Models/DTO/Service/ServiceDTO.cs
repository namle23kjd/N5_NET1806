﻿using PetSpa.Models.Domain;
using PetSpa.Models.DTO.BookingDetail;
using PetSpa.Models.DTO.Combo;
using System.ComponentModel.DataAnnotations;

namespace PetSpa.Models.DTO.Service
{
    public class ServiceDTO
    {
        public Guid ServiceId { get; set; }

        public string? ServiceName { get; set; }

        public bool Status { get; set; }

        public string? ServiceDescription { get; set; }

        public byte[]? ServiceImage { get; set; }
        [Required]
        public string? Duration { get; set; }
        public decimal Price { get; set; }
        public Guid? ComboId { get; set; }

        public virtual ICollection<BookingDetailDTO> BookingDetails { get; set; } = new List<BookingDetailDTO>();

        public virtual ComboDTO? Combo { get; set; }
    }
}
