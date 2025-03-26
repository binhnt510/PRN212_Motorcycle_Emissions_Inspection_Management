using System.Text;
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
using Microsoft.Win32;

namespace App_MotoEmissions
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class InspectionManager : Window
    {
        public InspectionManager()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;

            // Kiểm tra thông tin đăng nhập (ví dụ đơn giản)

            if (UserAccountDAO.CheckAccountWithEmailPass(email,UserAccountDAO.encryptPassword(password)))
            {
                var accout = UserAccountDAO.GetAccountWithEmail(email);
                SessionManager.UserAccount = accout;
                //DashboardUserWindow dashboard = new DashboardUserWindow();
               // dashboard.Show();
                CheckUserRoleAndRedirect(accout);
                this.Close();

            }
            else
            {
                MessageBox.Show("Email hoặc mật khẩu sai.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CheckUserRoleAndRedirect(UserAccount account)
        {
            switch (account.Role.ToLower())
            {
                case "quản trị viên":
                    DashboardAdminWindow adminWindow = new DashboardAdminWindow();
                    adminWindow.Show();
                    break;

                case "chủ phương tiện":
                    DashboardUserWindow userWindow = new DashboardUserWindow();
                    userWindow.Show();
                    break;

                case "cảnh sát giao thông":
                    PoliceCheckWindow policeWindow = new PoliceCheckWindow();
                    policeWindow.Show();
                    break;
                case "kiểm định viên":
                  //  PoliceCheckWindow policeWindow = new PoliceCheckWindow();
                 //   policeWindow.Show();
                    break;
                default:
                    MessageBox.Show("Tài khoản không có quyền truy cập.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
            }
        }


        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            // Đóng ứng dụng
            Application.Current.Shutdown();
        }
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow(); // Tạo một đối tượng của màn hình đăng ký
            registerWindow.Show(); // Hiển thị màn hình đăng ký
            this.Close(); // Đóng màn hình đăng nhập nếu cần
        }


    }
}