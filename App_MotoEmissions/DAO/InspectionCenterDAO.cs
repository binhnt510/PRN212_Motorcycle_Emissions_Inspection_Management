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

        public static InspectionCenter GetInspectionCenterById(int centerId)
        {
            using (PVehicleContext context = new PVehicleContext())
            {
                return context.InspectionCenters.Find(centerId);
            }
        }


        public static bool AddInspectionCenter(InspectionCenter center)
        {
            using (var context = new PVehicleContext())
            {
                context.InspectionCenters.Add(center);
                return context.SaveChanges() > 0;
            }
        }

        public static bool UpdateInspectionCenter(InspectionCenter center)
        {
            using (var context = new PVehicleContext())
            {
                var existing = context.InspectionCenters.Find(center.CenterId);
                if (existing == null) return false;

                existing.Name = center.Name;
                existing.Address = center.Address;
                existing.PhoneNumber = center.PhoneNumber;
                existing.Email = center.Email;

                return context.SaveChanges() > 0;
            }
        }

        public static bool DeleteInspectionCenter(int centerId)
        {
            using (var context = new PVehicleContext())
            {
                var center = context.InspectionCenters.Find(centerId);
                if (center == null) return false;

                context.InspectionCenters.Remove(center);
                return context.SaveChanges() > 0;
            }
        }
    }
}
