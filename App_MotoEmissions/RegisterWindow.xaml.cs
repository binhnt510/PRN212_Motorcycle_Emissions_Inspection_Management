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
using App_MotoEmissions.DAO;
using App_MotoEmissions.Models;
using Microsoft.Identity.Client;

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
                
                  MessageBox.Show("Vui lòng điền tất cả các thông tin.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                  return;
                
            }
            if (!(phone.Length==10) || !(phone[0]=='0'))
            {
                MessageBox.Show("Số điện thoại phải là 10 số và bắt đầu bằng số 0.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!(email.Contains(".")) || !(email.Contains("@")))
            {
                MessageBox.Show("Email không đúng định dạng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (UserAccountDAO.CheckEmailExist(email)){
                MessageBox.Show("Email đã tồn tại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            UserAccount newAccount = new UserAccount()
            {
                FullName = fullName,
                Email = email,
                PasswordHash = UserAccountDAO.encryptPassword(password),
                PhoneNumber = phone,
                Address = address,
                Role = "Owner"
            };
            UserAccountDAO.AddAccount(newAccount);
            // Thực hiện logic đăng ký (lưu vào cơ sở dữ liệu, v.v.)
            MessageBox.Show("Đăng ký tài khoản thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow loginWindow = new MainWindow(); // Tạo một đối tượng của màn hình đăng nhập
            loginWindow.Show(); // Hiển thị màn hình đăng nhập
            this.Close(); // Đóng màn hình đăng ký
        }
    }
}
