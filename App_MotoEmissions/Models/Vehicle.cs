using System;
using System.Collections.Generic;

namespace App_MotoEmissions.Models;

public partial class Vehicle
{
    public int VehicleId { get; set; }

    public int? OwnerId { get; set; }

    public string LicensePlate { get; set; } = null!;

    public string Brand { get; set; } = null!;

    public string Model { get; set; } = null!;

    public int ManufactureYear { get; set; }

    public string? FuelType { get; set; }
    public string EngineNumber { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Inspection> Inspections { get; set; } = new List<Inspection>();

    public virtual UserAccount? Owner { get; set; }

    public virtual ICollection<Violation> Violations { get; set; } = new List<Violation>();
}
