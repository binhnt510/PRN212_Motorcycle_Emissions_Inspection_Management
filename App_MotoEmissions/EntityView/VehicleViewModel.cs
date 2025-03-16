using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App_MotoEmissions.Models;

namespace App_MotoEmissions.EntityView
{
    class VehicleViewModel
    {
        public int Number { get; set; }
        public Vehicle Vehicle { get; set; }

        // Forward properties from Vehicle
        public string LicensePlate => Vehicle.LicensePlate;
        public string Brand => Vehicle.Brand;
        public string Model => Vehicle.Model;
        public int ManufactureYear => Vehicle.ManufactureYear;
        public string EngineNumber => Vehicle.EngineNumber;
        public string FuelType => Vehicle.FuelType;

    }
}
