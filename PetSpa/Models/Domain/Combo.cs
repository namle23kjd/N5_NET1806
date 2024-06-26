﻿using System;
using System.Collections.Generic;

namespace PetSpa.Models.Domain;

public partial class Combo
{
    public Guid ComboId { get; set; }
    public string ComboType { get; set; } = null!;
    public decimal Price { get; set; }
    public TimeSpan Duration { get; set; }
    public bool Status { get; set; }
    public virtual List<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();
    public virtual List<Service> Services { get; set; } = new List<Service>();
}
