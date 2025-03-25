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

namespace App_MotoEmissions
{
    /// <summary>
    /// Interaction logic for InspectionCenterWindow.xaml
    /// </summary>
    public partial class InspectionCenterWindow : UserControl
    {
        public ObservableCollection<InspectionCenter> InspectionCenters { get; set; }

        private InspectionCenter _selectedCenter;

        public InspectionCenterWindow()
        {
            InitializeComponent();
            LoadInspectionCenters();
            DataContext = this; // Gán DataContext để binding
        }

        private void LoadInspectionCenters()
        {
            InspectionCenters = new ObservableCollection<InspectionCenter>(InspectionCenterDAO.GetInspectionCenters());
            dgInspectionCenters.ItemsSource = InspectionCenters;
        }

        private void dgInspectionCenters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgInspectionCenters.SelectedItem is InspectionCenter selectedCenter)
            {
                _selectedCenter = selectedCenter;
                txtName.Text = selectedCenter.Name;
                txtAddress.Text = selectedCenter.Address;
                txtPhoneNumber.Text = selectedCenter.PhoneNumber;
                txtEmail.Text = selectedCenter.Email;
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtAddress.Text) ||
                string.IsNullOrWhiteSpace(txtPhoneNumber.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            var newCenter = new InspectionCenter
            {
                Name = txtName.Text,
                Address = txtAddress.Text,
                PhoneNumber = txtPhoneNumber.Text,
                Email = txtEmail.Text
            };

            if (InspectionCenterDAO.AddInspectionCenter(newCenter))
            {
                InspectionCenters.Add(newCenter);
                MessageBox.Show("Thêm thành công!");
            }
            else
            {
                MessageBox.Show("Lỗi khi thêm!");
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCenter == null)
            {
                MessageBox.Show("Chọn một trung tâm để sửa!");
                return;
            }

            _selectedCenter.Name = txtName.Text;
            _selectedCenter.Address = txtAddress.Text;
            _selectedCenter.PhoneNumber = txtPhoneNumber.Text;
            _selectedCenter.Email = txtEmail.Text;

            if (InspectionCenterDAO.UpdateInspectionCenter(_selectedCenter))
            {
                dgInspectionCenters.Items.Refresh();
                MessageBox.Show("Cập nhật thành công!");
            }
            else
            {
                MessageBox.Show("Lỗi khi cập nhật!");
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCenter == null)
            {
                MessageBox.Show("Chọn một trung tâm để xóa!");
                return;
            }

            if (MessageBox.Show("Bạn có chắc muốn xóa?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (InspectionCenterDAO.DeleteInspectionCenter(_selectedCenter.CenterId))
                {
                    InspectionCenters.Remove(_selectedCenter);
                    MessageBox.Show("Xóa thành công!");
                }
                else
                {
                    MessageBox.Show("Lỗi khi xóa!");
                }
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            txtName.Text = string.Empty;
            txtAddress.Text = string.Empty;
            txtPhoneNumber.Text = string.Empty;
            txtEmail.Text = string.Empty;
            _selectedCenter = null; // Xóa lựa chọn hiện tại
            dgInspectionCenters.SelectedItem = null; // Bỏ chọn dòng trong DataGrid
        }
    }
}
