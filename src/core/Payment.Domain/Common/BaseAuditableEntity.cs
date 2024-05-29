﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.Common
{
    public class BaseAuditableEntity
    {
        public string? CreateBy { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
        public string? LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
    }
}
