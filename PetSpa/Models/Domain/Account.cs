using System;
using System.Collections.Generic;

namespace PetSpa.Models.Domain;

public partial class Account
{
    public Guid AccId { get; set; }

    public string UserName { get; set; } = null!;

    public string PassWord { get; set; } = null!;

    public bool Status { get; set; }

    public string Role { get; set; } = null!;

    public string? ForgotPasswordToken { get; set; }

    public virtual Admin? Admin { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Manager? Manager { get; set; }

    public virtual Staff? Staff { get; set; }
}
