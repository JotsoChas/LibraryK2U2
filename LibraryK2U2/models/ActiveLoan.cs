using System;
using System.Collections.Generic;

namespace LibraryK2U2.models;

public partial class ActiveLoan
{
    public int LoanId { get; set; }

    public string BookTitle { get; set; } = null!;

    public string MemberName { get; set; } = null!;

    public DateOnly LoanDate { get; set; }

    public DateOnly DueDate { get; set; }
}