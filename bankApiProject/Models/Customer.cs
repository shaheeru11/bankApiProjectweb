using System;
using System.Collections.Generic;

namespace bankApiProject.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string? CustomerName { get; set; }

    public string? CustomerEmail { get; set; }

    public string? CustomerPassword { get; set; }

    public string? HashedCustomerName { get; set; }

    public string? HashedTokens { get; set; }

    public virtual ICollection<Admin> Admins { get; set; } = new List<Admin>();

    public virtual ICollection<Bank> Banks { get; set; } = new List<Bank>();
}
