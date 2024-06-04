using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace PetSpa.Models.Domain;

public partial class Customer
{
    public Guid CusId { get; set; }
    public string Id { get; set; } // Chúng ta đổi AccId thành Id để phù hợp với tên cột trong cơ sở dữ liệu
    public string FullName { get; set; } = null!;
    public string Gender { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string CusRank { get; set; } = null!;
    public Guid AdminId { get; set; } // Thêm thuộc tính này để phản ánh quan hệ với Admin

    public virtual IdentityUser User { get; set; } = null!; // Quan hệ với bảng AspNetUsers
    public virtual Admin Admins { get; set; } = null!; // Quan hệ với bảng Admin

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public virtual ICollection<Pet> Pets { get; set; } = new List<Pet>();
    public virtual ICollection<Voucher> Vouchers { get; set; } = new List<Voucher>();
}
