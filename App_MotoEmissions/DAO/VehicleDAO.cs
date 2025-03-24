using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App_MotoEmissions.Models;
using Microsoft.EntityFrameworkCore;

namespace App_MotoEmissions.DAO
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
            return context.Vehicles
             .Include(x => x.Owner)  // Giả sử bạn có bảng Roles liên kết với UserAccount
             .Where(x => x.Owner.Email == email && x.IsActive ==true && x.Owner.Role =="Chủ phương tiện").OrderByDescending(x => x.VehicleId)
             .ToList();
             

        }
        public static Vehicle GetVehicleById(int id)
        {
            using (var context = new PVehicleContext()) // Sử dụng `using` để đảm bảo giải phóng tài nguyên sau khi sử dụng
            {
                return context.Vehicles
                    .Where(x => x.VehicleId == id && x.IsActive ==true)
                    .FirstOrDefault(); // Trả về null nếu không tìm thấy kết quả
            }


        }
        public static Vehicle GetVehicleByLicensePlate(string lp)
        {
            PVehicleContext context = new PVehicleContext();
            return context.Vehicles
             .Where(x => x.LicensePlate == lp && x.IsActive == true).FirstOrDefault();


        }
        public static void AddVehicle(Vehicle vehicle)
        {
            PVehicleContext db = new PVehicleContext();
            db.Vehicles.Add(vehicle);
            db.SaveChanges();
        }
        public static void DeleteVehicle(int id)
        {

            using (PVehicleContext context = new PVehicleContext())
            {
                // Tìm xe cần cập nhật trong cơ sở dữ liệu (nếu xe chưa được lấy từ context)
                var vehicleToUpdate = context.Vehicles.FirstOrDefault(v => v.VehicleId ==id);

                if (vehicleToUpdate != null)
                {
                    // Thay đổi thuộc tính IsActive thành false
                    vehicleToUpdate.IsActive = false;

                    // Lưu thay đổi vào cơ sở dữ liệu
                    context.SaveChanges();
                }
                else
                {
                    // Xử lý khi không tìm thấy xe, nếu cần
                    throw new Exception("Vehicle not found");
                }
            }
        }


        public static void UpdateVehicle(int id, string lp, string engine, string brand, string model ,string fuel, int year)
        {

            using (PVehicleContext context = new PVehicleContext())
            {
                // Tìm xe cần cập nhật trong cơ sở dữ liệu (nếu xe chưa được lấy từ context)
                var vehicleToUpdate = context.Vehicles.FirstOrDefault(v => v.VehicleId == id);

                if (vehicleToUpdate != null)
                {
                    // Thay đổi thuộc tính IsActive thành false
                    vehicleToUpdate.EngineNumber = engine;
                    vehicleToUpdate.Brand = brand;
                    vehicleToUpdate.Model = model;
                    vehicleToUpdate.FuelType = fuel;
                    vehicleToUpdate.LicensePlate= lp;
                    vehicleToUpdate.ManufactureYear = year;

                    // Lưu thay đổi vào cơ sở dữ liệu
                    context.SaveChanges();
                }
                else
                {
                    // Xử lý khi không tìm thấy xe, nếu cần
                    throw new Exception("Vehicle not found");
                }
            }
        }
        public static bool CheckLicensePlateExist(string licensePlate)
        {
            PVehicleContext context = new PVehicleContext();
            var vehicle = context.Vehicles
             .Where(x => x.LicensePlate == licensePlate)
             .FirstOrDefault();  
            if (vehicle == null) { return false; }
            return true;
        }

    }
}
