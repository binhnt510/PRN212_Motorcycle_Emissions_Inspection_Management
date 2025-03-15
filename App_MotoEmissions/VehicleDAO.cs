using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App_MotoEmissions.Models;
using Microsoft.EntityFrameworkCore;

namespace App_MotoEmissions
{
    class VehicleDAO
    {
        public static List<Vehicle> GetVehicles()
        {
            PVehicleContext context = new PVehicleContext();
            return context.Vehicles.ToList();
        }
        public static List<Vehicle> GetVehicleByOwnerIEmail(string email)
        {
            PVehicleContext context = new PVehicleContext();
            var vehicle = context.Vehicles
             .Include(x => x.Owner)  // Giả sử bạn có bảng Roles liên kết với UserAccount
             .Where(x => x.Owner.Email == email)
             .ToList();
            return vehicle.ToList();

        }
    }
}
