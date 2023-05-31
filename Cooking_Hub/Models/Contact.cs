using System;
using System.Collections.Generic;

namespace Cooking_Hub.Models;

public partial class Contact
{
    public string ContactId { get; set; } = null!;

    public string ContactName { get; set; } = null!;

    public string ContactEmail { get; set; } = null!;

    public string ContactMessage { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string ContactSubject { get; set; } = null!;
}
