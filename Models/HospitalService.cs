using System;
using System.Collections.Generic;

namespace LifeLineApi.Models;

public partial class HospitalService
{
    public int HsId { get; set; }

    public int? HsHId { get; set; }

    public string? HsServices { get; set; }

    public virtual Hospital? HsH { get; set; }
}
