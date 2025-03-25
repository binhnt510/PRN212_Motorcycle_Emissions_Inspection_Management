using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App_MotoEmissions.Models;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace App_MotoEmissions.DAO
{
    class UserAccountDAO
    {

        public class UserAccountDto
        {
            public int UserId { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string Role { get; set; }
            public bool? IsActive { get; set; }
            public int? CenterId { get; set; }
            public string CenterName { get; set; }
        }

        public static List<UserAccount> GetAccounts()
        {
            PVehicleContext db = new PVehicleContext();
            return db.UserAccounts.ToList();
        }
        public static void AddAccount(UserAccount account)
        {
            PVehicleContext db = new PVehicleContext();
            db.UserAccounts.Add(account);
            db.SaveChanges();
        }
        public static string encryptPassword(string password)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2")); // Chuyển sang hex
                }
                return sb.ToString();
            }
        }
        public static bool CheckAccountWithEmailPass(string email, string pass)
        {
            PVehicleContext context = new PVehicleContext();
            var userAccount = context.UserAccounts
             .Where(x => x.Email == email && x.PasswordHash == pass)
             .FirstOrDefault();  // Trả về tài khoản đầu tiên khớp, hoặc null nếu không có
            if (userAccount == null) { return false; }
            return true;
        }
        public static bool CheckEmailExist(string email)
        {
            PVehicleContext context = new PVehicleContext();
            var userAccount = context.UserAccounts
             .Where(x => x.Email == email)
             .FirstOrDefault();  // Trả về tài khoản đầu tiên khớp, hoặc null nếu không có
            if (userAccount == null) { return false; }
            return true;
        }
        public static UserAccount GetAccountWithEmail(string email)
        {
            PVehicleContext context = new PVehicleContext();
            var userAccount = context.UserAccounts
             .Where(x => x.Email == email)
             .FirstOrDefault();  // Trả về tài khoản đầu tiên khớp, hoặc null nếu không có
            return userAccount;
        }


        public List<UserAccountDto> GetAccounts(int page, int pageSize)
        {
            using (var context = new PVehicleContext())
            {
                var query = context.UserAccounts
                    .Include(u => u.Center) // Join với bảng InspectionCenter
                    .OrderBy(u => u.UserId) // Sắp xếp theo ID (có thể thay đổi nếu cần)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(u => new UserAccountDto
                    {
                        UserId = u.UserId,
                        FullName = u.FullName,
                        Email = u.Email,
                        PhoneNumber = u.PhoneNumber,
                        Role = u.Role,
                        IsActive = u.IsActive,
                        CenterId = u.CenterId,
                        CenterName = u.Center != null ? u.Center.Name : "N/A"
                    })
                    .ToList();

                return query;
            }
        }


        public static int GetTotalAccounts()
        {
            using (PVehicleContext db = new PVehicleContext())
            {
                return db.UserAccounts.Count();
            }
        }

        public static List<UserAccount> SearchAccounts(string keyword)
        {
            using (PVehicleContext db = new PVehicleContext())
            {
                return db.UserAccounts
                         .Where(u => u.FullName.Contains(keyword) || u.Email.Contains(keyword))
                         .ToList();
            }
        }

        public static void UpdateAccount(UserAccount account)
        {
            using (PVehicleContext db = new PVehicleContext())
            {
                var existingAccount = db.UserAccounts.Find(account.UserId);
                if (existingAccount != null)
                {
                    existingAccount.FullName = account.FullName;
                    existingAccount.Email = account.Email;
                    existingAccount.PhoneNumber = account.PhoneNumber;
                    existingAccount.Address = account.Address;
                    existingAccount.Role = account.Role;
                    existingAccount.CenterId = account.CenterId;
                    db.SaveChanges();
                }
            }
        }

        public static void DeleteAccount(int userId)
        {
            using (PVehicleContext db = new PVehicleContext())
            {
                var account = db.UserAccounts.Find(userId);
                if (account != null)
                {
                    db.UserAccounts.Remove(account);
                    db.SaveChanges();
                }
            }
        }
    }
}
