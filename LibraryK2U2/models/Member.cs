using System;
using System.Collections.Generic;

namespace LibraryK2U2.models;

public partial class Member
{
    public int MemberId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public bool IsBlocked { get; set; }

    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
}