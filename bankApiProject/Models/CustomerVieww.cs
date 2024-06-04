using System;
using System.Collections.Generic;

namespace bankApiProject.Models;

public partial class CustomerVieww
{
    public int CustomerId { get; set; }

    public string? CustomerEmail { get; set; }

    public string? CustomerPassword { get; set; }

    public string? HashedCustomerName { get; set; }
}
