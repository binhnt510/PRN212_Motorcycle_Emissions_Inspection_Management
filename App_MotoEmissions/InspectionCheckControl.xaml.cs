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
using System.Windows.Navigation;
using System.Windows.Shapes;
using App_MotoEmissions.DAO;
using App_MotoEmissions.Models;

namespace App_MotoEmissions
{
    /// <summary>
    /// Interaction logic for InspectionCheckControl.xaml
    /// </summary>
    public partial class InspectionCheckControl : UserControl
    {
        public InspectionCheckControl()
        {
            InitializeComponent();
        }

        public void SearchVehicle(object sender, RoutedEventArgs e)
        {
            string licensePlate = txtLicensePlate.Text.Trim();
            if (string.IsNullOrEmpty(licensePlate))
            {
                MessageBox.Show("Vui lòng nhập biển số xe!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var vehicleInfo = InspectionDAO.GetInspectionInfoByPlate(licensePlate);
            if (vehicleInfo != null)
            {
                // Cập nhật thông tin phương tiện
                txtVehicleInfo.Text = $"Biển số: {vehicleInfo.LicensePlate}\n" +
                                      $"Hãng xe: {vehicleInfo.Brand} - {vehicleInfo.Model}\n" +
                                      $"Năm SX: {vehicleInfo.ManufactureYear}\n" +
                                      $"Số máy: {vehicleInfo.EngineNumber}\n" +
                                      $"Nhiên liệu: {vehicleInfo.FuelType}\n" +
                                      $"Chủ xe: {vehicleInfo.OwnerName} - {vehicleInfo.OwnerPhone} - {vehicleInfo.OwnerEmail}";

                // Cập nhật thông tin kiểm định
                txtInspectionInfo.Text = $"Ngày kiểm định: {(vehicleInfo.ScheduledDate.HasValue ? vehicleInfo.ScheduledDate.Value.ToString("dd/MM/yyyy") : "Chưa đăng ký")}\n" +
                                         $"Trạng thái: {vehicleInfo.Status}\n" +
                                         $"CO: {vehicleInfo.CO_Level?.ToString("F2") ?? "N/A"} - " +
                                         $"HC: {vehicleInfo.HC_Level?.ToString("F2") ?? "N/A"} - " +
                                         $"NOx: {vehicleInfo.NOx_Level?.ToString("F2") ?? "N/A"}\n" +
                                         $"Kết quả: {vehicleInfo.Result}\n" +
                                         $"Ngày kiểm tra: {(vehicleInfo.TestDate.HasValue ? vehicleInfo.TestDate.Value.ToString("dd/MM/yyyy") : "Chưa kiểm tra")}";

                // Hiển thị kết quả
                ResultPanel.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("Không tìm thấy thông tin kiểm định cho phương tiện này.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                // Xóa thông tin cũ và ẩn panel
                txtVehicleInfo.Text = "";
                txtInspectionInfo.Text = "";
                ResultPanel.Visibility = Visibility.Collapsed;
            }
        }



    }


}
