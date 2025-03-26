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
        public static void AddViolationReport(Violation report)
        {
            using (var context = new PVehicleContext())
            {
                context.Violations.Add(report);
                context.SaveChanges();
            }
        }

        public static List<Violation> GetViolationsByVehicle(int vehicleId)
        {
            using (var context = new PVehicleContext())
            {
                return context.Violations
                    .Include(v => v.Vehicle) // Đảm bảo có thông tin Vehicle
                    .Where(v => v.VehicleId == vehicleId)
                    .OrderByDescending(v => v.ViolationDate)
                    .ToList();
            }
        }


        public static List<Violation> GetViolationsByVehicle(int vehicleId, int pageNumber, int pageSize)
        {
            using (var context = new PVehicleContext())
            {
                return context.Violations
                    .Where(v => v.VehicleId == vehicleId)
                    .OrderByDescending(v => v.ViolationDate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            }
        }

        public static int GetTotalViolationsByVehicle(int vehicleId)
        {
            using (var context = new PVehicleContext())
            {
                return context.Violations.Count(v => v.VehicleId == vehicleId);
            }
        }

        public static Violation GetViolationById(int violationId)
        {
            using (var context = new PVehicleContext())
            {
                return context.Violations.FirstOrDefault(v => v.ViolationId == violationId);
            }
        }

        public static void UpdateViolation(Violation updatedViolation)
        {
            using (var context = new PVehicleContext())
            {
                var violation = context.Violations.FirstOrDefault(v => v.ViolationId == updatedViolation.ViolationId);
                if (violation != null)
                {
                    violation.ViolationDetails = updatedViolation.ViolationDetails;
                    violation.FineAmount = updatedViolation.FineAmount;
                    violation.Status = updatedViolation.Status;
                    context.SaveChanges();
                }
            }
        }
        public static List<Violation> GetAllViolations()
        {
            using (var context = new PVehicleContext())
            {
                return context.Violations
                              .Include(v => v.Vehicle) // Nạp thông tin xe liên quan
                              .ToList();
            }
        }


        public static void DeleteViolation(int violationId)
        {
            using (var context = new PVehicleContext())
            {
                var violation = context.Violations.FirstOrDefault(v => v.ViolationId == violationId);
                if (violation != null)
                {
                    context.Violations.Remove(violation);
                    context.SaveChanges(); //  Quan trọng: Lưu thay đổi
                }
            }
        }


    }
}
