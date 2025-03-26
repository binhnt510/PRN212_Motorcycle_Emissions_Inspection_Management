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
    /// <summary>
    /// Interaction logic for ViolationReportControl.xaml
    /// </summary>
    public partial class ViolationReportControl : UserControl
    {
        private ObservableCollection<Violation> Violations = new ObservableCollection<Violation>();
        private Violation selectedViolation;
        private int policeId;

        public ViolationReportControl(int policeId)
        {
            InitializeComponent();
            LoadViolations2();
            dgViolations.ItemsSource = Violations;
            this.policeId = policeId;
        }

        // Load danh sách vi phạm từ database
        private void LoadViolations()
        {
            var violations = ViolationDAO.GetViolationsByVehicle(1); // Đảm bảo ID xe đúng
            Violations = new ObservableCollection<Violation>(violations);
            dgViolations.ItemsSource = Violations; // Cập nhật lại DataGrid
        }
        // Load danh sách vi phạm từ database
        private void LoadViolations2()
        {
            var violations = ViolationDAO.GetAllViolations(); // Lấy toàn bộ danh sách
            Violations.Clear(); // Xóa dữ liệu cũ
            foreach (var violation in violations)
            {
                Violations.Add(violation);
            }
            dgViolations.ItemsSource = Violations; // Cập nhật DataGrid
        }

        // Tìm kiếm vi phạm theo biển số xe
        private void SearchViolations(object sender, RoutedEventArgs e)
        {
            string searchText = txtSearchLicensePlate.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchText))
            {
                // Nếu ô tìm kiếm trống, hiển thị lại tất cả vi phạm
                LoadViolations2();
                return;
            }

            // Lọc danh sách vi phạm theo biển số xe
            var filteredViolations = Violations
                .Where(v => v.Vehicle?.LicensePlate != null && v.Vehicle.LicensePlate.ToLower().Contains(searchText))
                .ToList();

            dgViolations.ItemsSource = filteredViolations;
        }

        // Thêm vi phạm mới
        private void AddViolation(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLicensePlate.Text) || string.IsNullOrWhiteSpace(txtDetails.Text) || string.IsNullOrWhiteSpace(txtFineAmount.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var violation = new Violation
            {
                VehicleId = 1, // Cần sửa cho phù hợp
                ViolationDetails = txtDetails.Text,
                FineAmount = decimal.Parse(txtFineAmount.Text),
                ViolationDate = DateTime.Now,
                Status = "Chưa xử lý",
                PoliceId = policeId
            };

            ViolationDAO.AddViolationReport(violation);
            LoadViolations2();
            ClearFields();

            MessageBox.Show("Thêm vi phạm thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Chọn vi phạm từ danh sách
        private void dgViolations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgViolations.SelectedItem is Violation violation)
            {
                selectedViolation = violation;
                txtLicensePlate.Text = violation.Vehicle?.LicensePlate ?? "Không xác định"; // Kiểm tra null
                txtDetails.Text = violation.ViolationDetails;
                txtFineAmount.Text = violation.FineAmount.ToString();
            }
        }


        // Sửa vi phạm
        private void UpdateViolation(object sender, RoutedEventArgs e)
        {
            if (selectedViolation == null)
            {
                MessageBox.Show("Vui lòng chọn một vi phạm để sửa!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Bạn có chắc chắn muốn cập nhật vi phạm này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                return;
            }

            selectedViolation.ViolationDetails = txtDetails.Text;
            selectedViolation.FineAmount = decimal.Parse(txtFineAmount.Text);
           

            ViolationDAO.UpdateViolation(selectedViolation);
            LoadViolations2();
            ClearFields();
        }

        // Xóa vi phạm
        private void DeleteViolation(object sender, RoutedEventArgs e)
        {
            if (selectedViolation == null)
            {
                MessageBox.Show("Vui lòng chọn một vi phạm để xóa!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa vi phạm này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.No)
            {
                return;
            }

            ViolationDAO.DeleteViolation(selectedViolation.ViolationId);
            LoadViolations2();
            ClearFields();
        }


        // Tải lại danh sách vi phạm
        private void ReloadViolations(object sender, RoutedEventArgs e)
        {
            LoadViolations2();
            ClearFields();
        }

        // Xóa dữ liệu trên form nhập
        private void ClearFields()
        {
            txtLicensePlate.Text = "";
            txtDetails.Text = "";
            txtFineAmount.Text = "";
            selectedViolation = null;
        }

    }
}
