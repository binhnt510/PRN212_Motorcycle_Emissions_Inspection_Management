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
    /// Interaction logic for DashboardAdminWindow.xaml
    /// </summary>
    public partial class DashboardAdminWindow : Window
    {
        public DashboardAdminWindow()
        {
            InitializeComponent();
            MainContent.Content = new ReportAndStatisticsWindow(); 
            UserAccount account = SessionManager.UserAccount;
            if (account != null)
            {
                string name = account.FullName;
                txtName.Text = name;
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new UserAccountsWindow();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new InspectionCenterWindow();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ReportAndStatisticsWindow();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            SessionManager.UserAccount = null;
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
