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
using Microsoft.Win32;

namespace App_MotoEmissions
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;

            // Kiểm tra thông tin đăng nhập (ví dụ đơn giản)

            if (UserAccountDAO.GetAccountWithEmailPass(email,UserAccountDAO.encryptPassword(password)))
            {
                MessageBox.Show("Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                UserHomeWindow userHomeWindow = new UserHomeWindow();  
                userHomeWindow.Show();

            }
            else
            {
                MessageBox.Show("Invalid email or password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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