using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App_MotoEmissions.Models
{
    internal class VehicleInspectionInfo
    {
        public string LicensePlate { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int ManufactureYear { get; set; }
        public string EngineNumber { get; set; }
        public string FuelType { get; set; }

        public string OwnerName { get; set; }
        public string OwnerEmail { get; set; }
        public string OwnerPhone { get; set; }

        public DateTime? ScheduledDate { get; set; }
        public string Status { get; set; }

        public double? CO_Level { get; set; }
        public double? HC_Level { get; set; }
        public double? NOx_Level { get; set; }
        public string Result { get; set; }
        public DateTime? TestDate { get; set; }
    }
}
