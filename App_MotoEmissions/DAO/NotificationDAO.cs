using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App_MotoEmissions.Models;
using Microsoft.EntityFrameworkCore;

namespace App_MotoEmissions.DAO
{
    class NotificationDAO
    {
        public static List<Notification> GetNotificationByOwner(UserAccount account)
        {
            PVehicleContext context = new PVehicleContext();
            return context.Notifications
             .Include(x=> x.User)
             .Where(x => x.UserId == account.UserId && x.User.Role == "Chủ phương tiện").OrderByDescending(x => x.SentDate)
             .ToList();


        }
        public static void CreateNotification(UserAccount user, string message)
        {
            using (var context = new PVehicleContext())
            {
                var notification = new Notification
                {
                    UserId = user.UserId,
                    Message = message,
                    SentDate = DateTime.Now
                };

                context.Notifications.Add(notification);
                context.SaveChanges();
            }
        }
    }
}
