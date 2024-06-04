using System;
using System.Collections.Generic;

namespace bankApiProject.Models;

public partial class Bank
{
    public int BankId { get; set; }

    public string? BankName { get; set; }

    public string? BankAccountNumber { get; set; }

    public bool? IsBankCreated { get; set; }

    public int? BankBalance { get; set; }

    public int? CustomerId { get; set; }

    public int? DepositAmount { get; set; }

    public int? WithdrawalAmount { get; set; }

    public virtual Customer? Customer { get; set; }
}
