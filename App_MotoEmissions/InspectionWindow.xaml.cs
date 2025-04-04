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

    namespace App_MotoEmissions
    {
        /// <summary>
        /// Interaction logic for InspectionWindow.xaml
        /// </summary>
        public partial class InspectionWindow : Window
        {

            private InspectionViewModel _viewModel;
            private System.Collections.Generic.List<InspectionViewModel.InspectionData> _allInspections;

            public InspectionWindow()
            {
           
            InitializeComponent();
                _viewModel = new InspectionViewModel(
                    "Server=LAPTOP-82HD84H0;uid=binhnt;password=123;database=P_Vehicle;Encrypt=True;TrustServerCertificate=True;"
                );
                SetupEventHandlers();
                LoadInspectionList();
            LoadInspectionList1();
        }

            private void LoadInspectionList(string status = null, DateTime? dateFilter = null)
            {
                // Lưu toàn bộ danh sách kiểm định
                _allInspections = _viewModel.GetInspectionList(status, dateFilter);

                // Cập nhật cả hai DataGrid với nguồn dữ liệu thống nhất
                inspectionDataGrid.ItemsSource = _allInspections;
                DataGridInspections.ItemsSource = _allInspections.Where(i => i.Status == "Chờ xác nhận").ToList();

            }
        private void LoadInspectionList1(string status = null, DateTime? dateFilter = null)
        {
            // Lưu toàn bộ danh sách kiểm định
            _allInspections = _viewModel.GetInspectionList(status, dateFilter);

            // Cập nhật cả hai DataGrid với nguồn dữ liệu thống nhất
            inspectionDataGrid.ItemsSource = _allInspections;
            DataGridInspections.ItemsSource = _allInspections.Where(i => i.Status == "Đã hoàn thành").ToList();

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
            private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                // Xác định DataGrid nào được chọn
                DataGrid currentGrid = sender as DataGrid;
                InspectionViewModel.InspectionData selectedInspection =
                    currentGrid.SelectedItem as InspectionViewModel.InspectionData;

                // Bỏ chọn ở DataGrid kia
                if (currentGrid == inspectionDataGrid)
                {
                    DataGridInspections.SelectedItem = null;
                }
                else if (currentGrid == DataGridInspections)
                {
                    inspectionDataGrid.SelectedItem = null;
                }

                // Hiển thị thông tin xe
                SelectInspection(selectedInspection);
            }

            private void btnShowInfo_Click(object sender, RoutedEventArgs e)
            {
                // Lấy xe được chọn từ bất kỳ DataGrid nào
                var selectedInspection =
                    inspectionDataGrid.SelectedItem as InspectionViewModel.InspectionData ??
                    DataGridInspections.SelectedItem as InspectionViewModel.InspectionData;

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
                var selectedInspection =
                    inspectionDataGrid.SelectedItem as InspectionViewModel.InspectionData ??
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
            private void Button_Click(object sender, RoutedEventArgs e)
            {
                SessionManager.UserAccount = null;
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            // Lấy dòng dữ liệu từ nút được click
            var button = sender as Button;
            var inspection = button?.DataContext as InspectionViewModel.InspectionData;

            if (inspection == null)
            {
                MessageBox.Show("Không thể xác định thông tin xe.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Lấy thông tin chủ xe từ dữ liệu kiểm định
                var userAccount = _viewModel.GetUserByVehicle(inspection.InspectionID);
                if (userAccount == null)
                {
                    MessageBox.Show("Không tìm thấy thông tin chủ xe.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Gửi thông báo
                string message = $"📢 Kiểm định xe {inspection.LicensePlate} ({inspection.Brand} {inspection.Model}) " +
                                 $"được lên lịch vào {inspection.ScheduledDate:dd/MM/yyyy HH:mm}. Trạng thái: {inspection.Status}";

                NotificationDAO.CreateNotification(userAccount, message);

                // Làm mới danh sách thông báo (nếu có thể truy cập NotificationWindow)
                if (Application.Current.Windows.OfType<NotificationWindow>().FirstOrDefault() is NotificationWindow notiWindow)
                {
                    notiWindow.ForceRefreshNotifications();
                }

                MessageBox.Show("✅ Đã gửi thông báo thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi khi gửi thông báo: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
