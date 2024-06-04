using System;
using System.Collections.Generic;

namespace PetSpa.Models.Domain;

public partial class BookingDetail
{
    public Guid BookingDetailId { get; set; } // ID của BookingDetail

    public Guid BookingId { get; set; } // ID của Booking

    public Guid PetId { get; set; } // ID của Pet

    public Guid? ServiceId { get; set; } // ID của Service (có thể null)

    public Guid? StaffId { get; set; } // ID của Staff (có thể null)

    public Guid? ComboId { get; set; } // ID của Combo (có thể null)

    public DateTime StartDate { get; set; } // Ngày bắt đầu

    public DateTime EndDate { get; set; } // Ngày kết thúc

    public string ComboType { get; set; } = null!; // Loại Combo

    public virtual Booking Booking { get; set; } = null!; // Quan hệ với bảng Booking

    public virtual Combo? Combo { get; set; } // Quan hệ với bảng Combo (có thể null)

    public virtual Pet Pet { get; set; } = null!; // Quan hệ với bảng Pet

    public virtual Service? Service { get; set; } // Quan hệ với bảng Service (có thể null)

    public virtual Staff? Staff { get; set; } // Quan hệ với bảng Staff (có thể null)
}
