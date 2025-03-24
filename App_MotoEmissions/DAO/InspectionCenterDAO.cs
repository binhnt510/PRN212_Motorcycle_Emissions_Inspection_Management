using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App_MotoEmissions.Models;

namespace App_MotoEmissions.DAO
{
    internal class InspectionCenterDAO
    {
        public static List<InspectionCenter> GetInspectionCenters()
        {
            PVehicleContext context = new PVehicleContext();
            return context.InspectionCenters.ToList();
        }
    }
}
