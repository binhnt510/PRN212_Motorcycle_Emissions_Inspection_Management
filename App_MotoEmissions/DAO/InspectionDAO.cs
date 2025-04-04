using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App_MotoEmissions.Models;
using Microsoft.Data.SqlClient;
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
                
                Inspection inspection = context.Inspections.FirstOrDefault(v => v.InspectionId == id);

                if (inspection != null)
                {
                    // Thay đổi thuộc tính IsActive thành false
                    context.Remove(inspection);
                    // Lưu thay đổi vào cơ sở dữ liệu
                    context.SaveChanges();
                }
                
            }
        }

        public static VehicleInspectionInfo GetInspectionInfoByPlate(string licensePlate)
        {
            using (var context = new PVehicleContext())
            {
                var result = (from v in context.Vehicles
                              join u in context.UserAccounts on v.OwnerId equals u.UserId
                              join i in context.Inspections on v.VehicleId equals i.VehicleId into inspections
                              from i in inspections.DefaultIfEmpty()
                              join e in context.EmissionTests on i.InspectionId equals e.InspectionId into emissionTests
                              from e in emissionTests.DefaultIfEmpty()
                              where v.LicensePlate == licensePlate
                              orderby i != null ? i.ScheduledDate : DateTime.MinValue descending // Tránh lỗi null khi sắp xếp
                              select new VehicleInspectionInfo
                              {
                                  LicensePlate = v.LicensePlate,
                                  Brand = v.Brand,
                                  Model = v.Model,
                                  ManufactureYear = v.ManufactureYear,
                                  EngineNumber = v.EngineNumber,
                                  FuelType = v.FuelType,
                                  OwnerName = u.FullName,
                                  OwnerEmail = u.Email,
                                  OwnerPhone = u.PhoneNumber,
                                  ScheduledDate = i != null ? i.ScheduledDate : (DateTime?)null,
                                  Status = i != null ? i.Status : "Chưa đăng ký",
                                  CO_Level = e != null ? e.CoLevel : (double?)null,
                                  HC_Level = e != null ? e.HcLevel : (double?)null,
                                  NOx_Level = e != null ? e.NoxLevel : (double?)null,
                                  Result = e != null ? e.Result : "Chưa kiểm tra",
                                  TestDate = e != null ? e.TestDate : (DateTime?)null
                              }).FirstOrDefault();

                return result;
            }
        }

      
    }
}
