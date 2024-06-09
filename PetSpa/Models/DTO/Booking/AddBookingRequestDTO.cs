using PetSpa.Models.DTO.BookingDetail;
using System;
using System.Collections.Generic;

namespace PetSpa.Models.DTO.Booking
{
    public class AddBookingRequestDTO
    {
        public Guid CusId { get; set; }
        public DateTime BookingSchedule { get; set; }
        public List<AddBookingDetailRequestDTO> BookingDetails { get; set; } = new List<AddBookingDetailRequestDTO>();
    }
}
