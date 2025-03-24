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
using System.Windows.Shapes;
using App_MotoEmissions.Models;

namespace App_MotoEmissions
{
    /// <summary>
    /// Interaction logic for DashboardUserWindow.xaml
    /// </summary>
    public partial class DashboardUserWindow : Window
    {
        public DashboardUserWindow()
        {
            InitializeComponent();
            MainContent.Content = new NotificationWindow(); // Load mặc định màn 1
            UserAccount account = SessionManager.UserAccount;
            if (account != null)
            {
                string name = account.FullName;
                txtName.Text = name;
            }
            
        }

        private void ScreenNoti_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new NotificationWindow(); // Load dữ liệu mỗi khi mở màn
        }

        private void ScreenVehi_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new VehicleManagementWindow();
        }
        private void ScreenInspecClick(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new InspectionManagementWindow();
        }

        private void LogoutClick(object sender, RoutedEventArgs e)
        {
            SessionManager.UserAccount=null; 
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
