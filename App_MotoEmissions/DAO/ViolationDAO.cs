using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App_MotoEmissions.Models;
using Microsoft.EntityFrameworkCore;

namespace App_MotoEmissions.DAO
{
    class ViolationDAO
    {
        public static List<Violation> GetViolations(string email)
        {
            PVehicleContext db = new PVehicleContext();
            return db.Violations.Include(x => x.Vehicle).ThenInclude(y => y.Owner).Where(x => x.Vehicle.Owner.Email == email)
                .OrderByDescending(x => x.ViolationDate)
                .ToList();
        }
    }
}
