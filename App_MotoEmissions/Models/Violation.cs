using System;
using System.Collections.Generic;

namespace App_MotoEmissions.Models;

public partial class Violation
{
    public int ViolationId { get; set; }

    public int? VehicleId { get; set; }

    public int? PoliceId { get; set; }

    public DateTime? ViolationDate { get; set; }

    public string ViolationDetails { get; set; } = null!;

    public decimal? FineAmount { get; set; }

    public string Status { get; set; } = null!;

    public virtual UserAccount? Police { get; set; }

    public virtual Vehicle? Vehicle { get; set; }
    public bool? PayFine { get; set; }
}
