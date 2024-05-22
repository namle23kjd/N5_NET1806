using System;
using System.Collections.Generic;

namespace PetSpa.Models.Domain;

public partial class Admin
{
    public Guid AccId { get; set; }

    public Guid AdminId { get; set; }

    public virtual Account Acc { get; set; } = null!;
}
