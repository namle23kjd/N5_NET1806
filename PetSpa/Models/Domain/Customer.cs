using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetSpa.Models.Domain
{
    public partial class Customer
    {
        public Guid CusId { get; set; }
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string CusRank { get; set; } = null!;
        public virtual List<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual List<Booking> Bookings { get; set; } = new List<Booking>();
        public virtual List<Pet> Pets { get; set; } = new List<Pet>();
    }

}
