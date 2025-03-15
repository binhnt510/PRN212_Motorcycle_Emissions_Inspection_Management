using System;
using System.Collections.Generic;

namespace App_MotoEmissions.Models;

public partial class Inspection
{
    public int InspectionId { get; set; }

    public int? VehicleId { get; set; }

    public int? CenterId { get; set; }

    public int? InspectorId { get; set; }

    public DateTime ScheduledDate { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual InspectionCenter? Center { get; set; }

    public virtual ICollection<EmissionTest> EmissionTests { get; set; } = new List<EmissionTest>();

    public virtual UserAccount? Inspector { get; set; }

    public virtual Vehicle? Vehicle { get; set; }
}
