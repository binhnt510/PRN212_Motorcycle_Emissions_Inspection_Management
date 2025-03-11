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

namespace App_MotoEmissions
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string fullName = FullNameTextBox.Text;
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;
            string phone = PhoneTextBox.Text;
            string address = AddressTextBox.Text;

            // Kiểm tra thông tin đăng ký (ví dụ đơn giản)
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(address))
            {
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Thực hiện logic đăng ký (lưu vào cơ sở dữ liệu, v.v.)
            MessageBox.Show("Registration successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow loginWindow = new MainWindow(); // Tạo một đối tượng của màn hình đăng nhập
            loginWindow.Show(); // Hiển thị màn hình đăng nhập
            this.Close(); // Đóng màn hình đăng ký
        }
    }
}
