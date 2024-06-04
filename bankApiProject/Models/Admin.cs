using System;
using System.Collections.Generic;

namespace bankApiProject.Models;

public partial class Admin
{
    public int AdminId { get; set; }

    public string? AdminUsername { get; set; }

    public string? AdminPassword { get; set; }

    public int? BankId { get; set; }

    public int? CustomerId { get; set; }

    public virtual Customer? Customer { get; set; }
}
