using System;
using System.Collections.Generic;

namespace PetSpa.Models.Domain;

public partial class Payment
{
    public Guid PayId { get; set; }

    public Guid InvoiceId { get; set; }

    public virtual Invoice Invoice { get; set; } = null!;
}
