using System;
using System.Collections.Generic;

namespace PetSpa.Models.Domain;

public partial class Payment
{
    public string Id { get; set; }
    public string? PaymentContent { get; set; }
    public string? PaymentCurrency { get; set; }
    public string? PaymentRefId { get; set; }
    public decimal RequiredAmount { get; set; }
    public DateTime PaymentDate { get; set; }
    public DateTime ExpireDate { get; set; }
    public string? PaymentLanguage { get; set; }
    public string? MerchantId { get; set; }
    public string? PaymentDestinationId { get; set; }
    public decimal PaidAmount { get; set; }
    public string? PaymentStatus { get; set; }
    public string? PaymentLastMessage { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? InvoiceId { get; set; }

    public Invoice? Invoice { get; set; }
    public Merchant? Merchant { get; set; }
}
