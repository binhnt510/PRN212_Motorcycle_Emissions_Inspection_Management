using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using App_MotoEmissions.DAO;
using App_MotoEmissions.Models;

namespace App_MotoEmissions
{
    /// <summary>
    /// Interaction logic for NotificationWindow.xaml
    /// </summary>
    public partial class NotificationWindow : UserControl
    {
        public NotificationWindow()
        {
            InitializeComponent();
            LoadDataGridViolations();
            LoadDataGridNotifications();
        }

        private void RefreshData(object sender, RoutedEventArgs e)
        {
            LoadDataGridViolations();
            LoadDataGridNotifications();

        }
        void LoadDataGridViolations()
        {
            UserAccount? acc = SessionManager.UserAccount;
            var violation = ViolationDAO.GetViolations(acc.Email);
            this.dataGridViolations.ItemsSource = violation;
        }
        void LoadDataGridNotifications()
        {
            UserAccount? acc = SessionManager.UserAccount;
            var noti = NotificationDAO.GetNotificationByOwner(acc);
            this.dataGridNotifications.ItemsSource = noti;
        }
    }
}
