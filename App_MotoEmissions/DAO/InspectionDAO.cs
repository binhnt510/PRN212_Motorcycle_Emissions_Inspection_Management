using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App_MotoEmissions.Models;
using Microsoft.EntityFrameworkCore;

namespace App_MotoEmissions.DAO
{
    internal class InspectionDAO
    {
        public static void AddInspection(Inspection inspection)
        {
            PVehicleContext db = new PVehicleContext();
            db.Inspections.Add(inspection);
            db.SaveChanges();
        }
        public static List<Inspection> GetInspectionByOwnerIEmail(string email)
        {
            PVehicleContext context = new PVehicleContext();
            return context.Inspections
            .Include(i => i.Vehicle)
                .ThenInclude(v => v.Owner)
            .Include(i => i.Center) // Liên kết với bảng InspectionCenter
            .Where(i => i.Vehicle.Owner.Email == email
                     && i.Vehicle.IsActive == true
                     && i.Vehicle.Owner.Role == "Chủ phương tiện")
            .OrderByDescending(i => i.ScheduledDate)
            .ToList();


        }
        public static bool  GetInspectionById(int id)
        {
            PVehicleContext context = new PVehicleContext();
            var inp = context.Inspections
             .Where(x => x.InspectionId == id && x.Status == "Chờ xác nhận")
             .FirstOrDefault();
            if (inp == null) { return false; }
            return true;

        }
        public static bool GetInspectionByVehicle(int vid)
        {
            PVehicleContext context = new PVehicleContext();
            var inp = context.Inspections
                .Include(x => x.Vehicle)
             .Where(x => x.Vehicle.VehicleId == vid && x.Status == "Chờ xác nhận")
             .FirstOrDefault();
            if (inp == null) { return false; }
            return true;

        }
        public static void DeleteInspection(int id)
        {

            using (PVehicleContext context = new PVehicleContext())
            {
                
                var inspection = context.Inspections.FirstOrDefault(v => v.InspectorId == id);

                if (inspection != null)
                {
                    // Thay đổi thuộc tính IsActive thành false
                    context.Remove(inspection);
                    // Lưu thay đổi vào cơ sở dữ liệu
                    context.SaveChanges();
                }
                
            }
        }
    }
}
