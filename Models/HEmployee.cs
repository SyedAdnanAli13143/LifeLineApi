using System;
using System.Collections.Generic;

namespace LifeLineApi.Models;

public partial class HEmployee
{
    public int HeId { get; set; }

    public int? HeHId { get; set; }

    public string? HeName { get; set; }

    public string? HeEmail { get; set; }

    public string? HePassword { get; set; }

    public string? HeRole { get; set; }

    public virtual Hospital? HeH { get; set; }
}
