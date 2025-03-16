using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App_MotoEmissions.Models;
using System.Security.Cryptography;

namespace App_MotoEmissions
{
    class UserAccountDAO
    {
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
        public static bool CheckAccountWithEmailPass( string email, string pass)
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
             .Where(x => x.Email == email )
             .FirstOrDefault();  // Trả về tài khoản đầu tiên khớp, hoặc null nếu không có
            return userAccount;
        }

    }
}
