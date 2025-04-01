using System;
using System.Collections.Generic;

namespace OfficeFurnitureStore.Models​;

public partial class Partner
{
    public int PartnerId { get; set; }

    public string PartnerName { get; set; } = null!;

    public string? ContactPerson { get; set; }

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Address { get; set; }
}
