using System;
using System.Collections.Generic;

namespace PetSpa.Models.Domain;

public class PaymentT
{
    public Guid Id { get; set; }
    public Guid InvoiceId { get; set; } // Thêm trường InvoiceId
    public decimal RequiredAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public string? Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid MerchantId { get; set; } // MerchantId nếu có quan hệ với Merchant

    public virtual Invoice? Invoice { get; set; } // Quan hệ với bảng Invoice
    public virtual Merchant? Merchant { get; set; } // Quan hệ với bảng Merchant nếu có
}
