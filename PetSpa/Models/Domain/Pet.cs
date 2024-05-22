using System;
using System.Collections.Generic;

namespace PetSpa.Models.Domain;

public partial class Pet
{
    public Guid PetId { get; set; }

    public Guid CusId { get; set; } 

    public string PetType { get; set; } = null!;

    public string PetName { get; set; } = null!;

    public byte[]? Image { get; set; }

    public bool Status { get; set; }

    public DateOnly? PetBirthday { get; set; }

    public decimal? PetWeight { get; set; }

    public decimal? PetHeight { get; set; }

    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();

    public virtual Customer Cus { get; set; } = null!;
}
