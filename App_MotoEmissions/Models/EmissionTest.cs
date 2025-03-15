using System;
using System.Collections.Generic;

namespace App_MotoEmissions.Models;

public partial class EmissionTest
{
    public int TestId { get; set; }

    public int? InspectionId { get; set; }

    public double? CoLevel { get; set; }

    public double? HcLevel { get; set; }

    public double? NoxLevel { get; set; }

    public string? Result { get; set; }

    public DateTime? TestDate { get; set; }

    public virtual Inspection? Inspection { get; set; }
}
