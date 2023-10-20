using System;
using System.Collections.Generic;

namespace LifeLineApi.Models;

public partial class ApplicationAdmin
{
    public int AaId { get; set; }

    public string? AaUserName { get; set; }

    public string? AaEmail { get; set; }

    public string? AaPassword { get; set; }
}
