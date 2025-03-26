using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Microsoft.EntityFrameworkCore;

namespace App_MotoEmissions
{
    public partial class UserAccountsWindow : UserControl
    {
        private ObservableCollection<UserAccount> Users;
        private ObservableCollection<InspectionCenter> InspectionCenters;
        private ObservableCollection<UserAccount> allUsers = new ObservableCollection<UserAccount>();
        private ObservableCollection<UserAccount> filteredUsers = new ObservableCollection<UserAccount>();

        private UserAccount selectedUser;
        private readonly UserAccountDAO userAccountDAO = new UserAccountDAO();
        private const int PageSize = 10;
        private int totalPages;
        private int currentPage = 1;

        public UserAccountsWindow()
        {
            InitializeComponent();
            totalPages = userAccountDAO.GetTotalPages(PageSize);
            LoadUsers();
            LoadInspectionCenters();
        }

        private void LoadUsers()
        {
            using (var context = new PVehicleContext())
            {
                totalPages = userAccountDAO.GetTotalPages(PageSize);

                var users = context.UserAccounts
                                   .Include(u => u.Center)
                                   .OrderBy(u => u.UserId)
                                   .Skip((currentPage - 1) * PageSize)
                                   .Take(PageSize)
                                   .Select(u => new
                                   {
                                       u.UserId,
                                       u.FullName,
                                       u.Email,
                                       u.PhoneNumber,
                                       u.Address,
                                       u.Role,
                                       u.CenterId,
                                       CenterName = u.Center != null ? u.Center.Name : "N/A"
                                   })
                                   .ToList();

                Users = new ObservableCollection<UserAccount>(
                    users.Select(u => new UserAccount
                    {
                        UserId = u.UserId,
                        FullName = u.FullName,
                        Email = u.Email,
                        PhoneNumber = u.PhoneNumber,
                        Address = u.Address,
                        Role = u.Role,
                        CenterId = u.CenterId,
                        CenterName = u.CenterName
                    })
                );

                allUsers = new ObservableCollection<UserAccount>(Users);
                dgUsers.ItemsSource = Users;
            }
        }

        private void LoadInspectionCenters()
        {
            InspectionCenters = new ObservableCollection<InspectionCenter>(InspectionCenterDAO.GetInspectionCenters());
            cbInspectionCenter.ItemsSource = InspectionCenters;
            cbInspectionCenter.DisplayMemberPath = "Name";
        }

        private void SearchUsers(object sender, RoutedEventArgs e)
        {
            string nameKeyword = txtSearchName.Text.Trim().ToLower();
            string phoneKeyword = txtSearchPhone.Text.Trim();
            string emailKeyword = txtSearchEmail.Text.Trim().ToLower();

            filteredUsers = new ObservableCollection<UserAccount>(
                allUsers.Where(u => (string.IsNullOrEmpty(nameKeyword) || u.FullName.ToLower().Contains(nameKeyword)) &&
                                    (string.IsNullOrEmpty(phoneKeyword) || u.PhoneNumber.Contains(phoneKeyword)) &&
                                    (string.IsNullOrEmpty(emailKeyword) || u.Email.ToLower().Contains(emailKeyword)))
            );

            dgUsers.ItemsSource = filteredUsers;
        }

        private void ResetSearch(object sender, RoutedEventArgs e)
        {
            txtSearchName.Text = "";
            txtSearchPhone.Text = "";
            txtSearchEmail.Text = "";
            dgUsers.ItemsSource = allUsers;
        }

        private void AddUser(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text) || string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtPhone.Text) || string.IsNullOrWhiteSpace(txtPassword.Password) ||
                string.IsNullOrWhiteSpace(txtAddress.Text) || cbRole.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newUser = new UserAccount
            {
                FullName = txtFullName.Text,
                Email = txtEmail.Text,
                PhoneNumber = txtPhone.Text,
                PasswordHash = UserAccountDAO.encryptPassword(txtPassword.Password),
                Address = txtAddress.Text,
                Role = (cbRole.SelectedItem as ComboBoxItem)?.Content.ToString(),
                CenterId = cbInspectionCenter.SelectedItem is InspectionCenter center ? center.CenterId : (int?)null
            };

            UserAccountDAO.AddAccount(newUser);
            MessageBox.Show("Thêm tài khoản thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadUsers();
            ResetForm();
        }

        private void UpdateUser(object sender, RoutedEventArgs e)
        {
            if (selectedUser == null)
            {
                MessageBox.Show("Vui lòng chọn một tài khoản để sửa!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            selectedUser.FullName = txtFullName.Text;
            selectedUser.Email = txtEmail.Text;
            selectedUser.PhoneNumber = txtPhone.Text;
            selectedUser.Address = txtAddress.Text;
            selectedUser.Role = (cbRole.SelectedItem as ComboBoxItem)?.Content.ToString();
            selectedUser.CenterId = cbInspectionCenter.SelectedItem is InspectionCenter center ? center.CenterId : (int?)null;

            UserAccountDAO.UpdateAccount(selectedUser);
            MessageBox.Show("Cập nhật tài khoản thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadUsers();
            ResetForm();
        }

        private void ResetForm()
        {
            txtFullName.Text = "";
            txtEmail.Text = "";
            txtPhone.Text = "";
            txtPassword.Password = "";
            txtAddress.Text = "";
            cbRole.SelectedIndex = -1;
            cbInspectionCenter.SelectedIndex = -1;
            selectedUser = null;
        }

        private void dgUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgUsers.SelectedItem is UserAccount user)
            {
                selectedUser = user;
                txtFullName.Text = user.FullName;
                txtEmail.Text = user.Email;
                txtPhone.Text = user.PhoneNumber;
                txtAddress.Text = user.Address;
                cbRole.SelectedItem = cbRole.Items.Cast<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == user.Role);

                // Tìm cơ sở kiểm định phù hợp với CenterId
                cbInspectionCenter.SelectedItem = InspectionCenters.FirstOrDefault(ic => ic.CenterId == user.CenterId);

                // Kiểm tra vai trò để hiển thị cơ sở kiểm định
                bool isInspector = user.Role == "Kiểm định viên";
                lblInspectionCenter.Visibility = isInspector ? Visibility.Visible : Visibility.Collapsed;
                cbInspectionCenter.Visibility = isInspector ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void cbRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbRole.SelectedItem is ComboBoxItem selectedRole)
            {
                bool isInspector = selectedRole.Content.ToString() == "Kiểm định viên";
                lblInspectionCenter.Visibility = isInspector ? Visibility.Visible : Visibility.Collapsed;
                cbInspectionCenter.Visibility = isInspector ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void ResetForm(object sender, RoutedEventArgs e)
        {
            ResetForm();
        }

        private void DeleteUser(object sender, RoutedEventArgs e)
        {
            if (selectedUser == null)
            {
                MessageBox.Show("Vui lòng chọn một tài khoản để xóa!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa tài khoản này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                UserAccountDAO.DeleteAccount(selectedUser.UserId);
                MessageBox.Show("Xóa tài khoản thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadUsers();
                ResetForm();
            }
        }
        private void NextPage(object sender, RoutedEventArgs e)
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                LoadUsers();
            }
        }

        private void PrevPage(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                LoadUsers();
            }
        }
    }
}
