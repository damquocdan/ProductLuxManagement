using System;
using System.Collections.Generic;

namespace OfficeFurnitureStore.Models​;

public partial class Contact
{
    public int ContactId { get; set; }

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Subject { get; set; } = null!;

    public string Message { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }
}
