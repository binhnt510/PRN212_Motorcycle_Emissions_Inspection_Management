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
    /// Interaction logic for InspectionWindow.xaml
    /// </summary>
    public partial class InspectionWindow : Window
    {
        private InspectionViewModel _viewModel;

        public InspectionWindow()
        {
            InitializeComponent();
            _viewModel = new InspectionViewModel(
                "Server=VIVOBOOK_PRO_15;uid=sa;password=123;database=P_Vehicle;Encrypt=True;TrustServerCertificate=True;"
            );
            SetupEventHandlers();
            LoadInspectionList();
        }

        private void LoadInspectionList(string status = null, DateTime? dateFilter = null)
        {
            var danhSachKiểmĐịnh = _viewModel.GetInspectionList(status, dateFilter);
            inspectionDataGrid.ItemsSource = danhSachKiểmĐịnh;
            DataGridInspections.ItemsSource = danhSachKiểmĐịnh.FindAll(i => i.Status == "Chờ xác nhận");
        }

        private void SetupEventHandlers()
        {
            statusComboBox.SelectionChanged += (s, e) => ApplyFilters();
            datePicker.SelectedDateChanged += (s, e) => ApplyFilters();
        }

        private void ApplyFilters()
        {
            string selectedStatus = (statusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (selectedStatus == "Tất cả") selectedStatus = null;
            LoadInspectionList(selectedStatus, datePicker.SelectedDate);
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e) => ApplyFilters();

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            statusComboBox.SelectedIndex = 0;
            datePicker.SelectedDate = null;
            LoadInspectionList();
        }

        private void SubmitInspectionResult_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCO.Text) || string.IsNullOrWhiteSpace(txtHC.Text) || string.IsNullOrWhiteSpace(txtNOx.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin kiểm định!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            MessageBox.Show("Báo cáo đã được gửi thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DataGridInspections_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectInspection(DataGridInspections.SelectedItem as InspectionViewModel.InspectionData);
        }

        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            SelectInspection((sender as Button)?.DataContext as InspectionViewModel.InspectionData);
        }

        private void SelectInspection(InspectionViewModel.InspectionData inspection)
        {
            if (inspection != null)
            {
                DisplayVehicleDetails(inspection);
                MainTabControl.SelectedIndex = 1;
            }
        }

        private void DisplayVehicleDetails(InspectionViewModel.InspectionData inspection)
        {
            txtRegistrationCode.Text = inspection.InspectionID.ToString();
            txtLicensePlate.Text = inspection.LicensePlate;
            txtOwnerName.Text = inspection.OwnerName;
            txtVehicleBrand.Text = inspection.Brand;
            txtVehicleModel.Text = inspection.Model;
            txtScheduledDate.Text = inspection.ScheduledDate.ToString("dd/MM/yyyy HH:mm");
        }

        private void btnShowInfo_Click(object sender, RoutedEventArgs e)
        {
            var selectedInspection = DataGridInspections.SelectedItem as InspectionViewModel.InspectionData ??
                                     inspectionDataGrid.SelectedItem as InspectionViewModel.InspectionData;

            if (selectedInspection == null)
            {
                MessageBox.Show("Vui lòng chọn một phương tiện để xem thông tin.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var emissionData = _viewModel.GetEmissionTestByInspectionId(selectedInspection.InspectionID);
            if (emissionData != null)
            {
                MessageBox.Show($"Thông tin chi tiết kiểm định:\n\n" +
                                $"Biển số xe: {selectedInspection.LicensePlate}\n" +
                                $"Chủ xe: {selectedInspection.OwnerName}\n" +
                                $"Ngày kiểm định: {selectedInspection.ScheduledDate:dd/MM/yyyy HH:mm}\n\n" +
                                $"Kết quả kiểm tra khí thải:\n" +
                                $"CO: {emissionData.CO} %\n" +
                                $"HC: {emissionData.HC} ppm\n" +
                                $"NOx: {emissionData.NOx} ppm\n" +
                                $"Ghi chú: {emissionData.Notes}\n" +
                                $"Ngày kiểm định: {emissionData.CreatedAt:dd/MM/yyyy HH:mm}",
                                "Chi tiết kiểm định", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Chưa có dữ liệu kiểm định cho phương tiện này.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SaveInspectionResult_Click(object sender, RoutedEventArgs e)
        {
            var selectedInspection = inspectionDataGrid.SelectedItem as InspectionViewModel.InspectionData ??
                                     DataGridInspections.SelectedItem as InspectionViewModel.InspectionData;

            if (selectedInspection == null)
            {
                MessageBox.Show("Vui lòng chọn một kiểm định để lưu.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!double.TryParse(txtCO.Text, out double co) ||
                !double.TryParse(txtHC.Text, out double hc) ||
                !double.TryParse(txtNOx.Text, out double nox))
            {
                MessageBox.Show("Vui lòng nhập giá trị hợp lệ cho CO, HC, NOx.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string status = rbPass.IsChecked == true ? "Đạt" : "Không đạt";
            string notes = txtNotes.Text;

            if (_viewModel.SubmitInspectionResult(selectedInspection.InspectionID, co, hc, nox, status, notes))
            {
                MessageBox.Show("Đã lưu kết quả kiểm định!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadInspectionList();
            }
            else
            {
                MessageBox.Show("Lưu thất bại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
