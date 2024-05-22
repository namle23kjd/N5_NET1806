using System;
using System.Collections.Generic;

namespace PetSpa.Models.Domain;

public partial class Payment
{
    public int PayId { get; set; }

    public int InvoiceId { get; set; }

    public virtual Invoice Invoice { get; set; } = null!;
}
