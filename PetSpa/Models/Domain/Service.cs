﻿using System;
using System.Collections.Generic;

namespace PetSpa.Models.Domain;

public partial class Service
{
    public Guid ServiceId { get; set; }
    public string? ServiceName { get; set; }
    public bool Status { get; set; }
    public string? ServiceDescription { get; set; }
    public byte[]? ServiceImage { get; set; }
    public TimeSpan Duration { get; set; }
    public decimal Price { get; set; }
    public Guid? ComboId { get; set; }
    public int? Points { get; set; }
    public virtual List<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();
    public virtual Combo? Combo { get; set; }
}

