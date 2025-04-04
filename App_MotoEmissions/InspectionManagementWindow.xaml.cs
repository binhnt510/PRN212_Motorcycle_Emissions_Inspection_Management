using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System;
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
using Microsoft.IdentityModel.Tokens;
using static System.Net.Mime.MediaTypeNames;

namespace App_MotoEmissions
{
    /// <summary>
    /// Interaction logic for InspectionManagementWindow.xaml
    /// </summary>
    public partial class InspectionManagementWindow : UserControl
    {
        public InspectionManagementWindow()
        {
            InitializeComponent();
            LoadComboboxCenter();
            LoadComboboxVehicle();
            LoadDataGridInspections();


            }

        private void CancelRegistration(object sender, RoutedEventArgs e)
        {
            string id = this.txtid.Text;
            if (id.IsNullOrEmpty()) 
            
            {
                MessageBox.Show("Vui lòng chọn 1 đăng ký", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!InspectionDAO.GetInspectionById(int.Parse(id)))
            {
                MessageBox.Show("Trạng thái đã Xác nhận", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            //MessageBox.Show($"{accountId} - {password} - {roleId}");
            if (MessageBox.Show("Bạn có chắc chắn muốn hủy lịch kiểm định?",
                    "Xác nhận",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                InspectionDAO.DeleteInspection(int.Parse(id));
                ClearRegistration(sender, e);
                LoadDataGridInspections();
            }
        }
        private void ClearRegistration(object sender, RoutedEventArgs e)
        {
            txtid.Text = "";
            txtBrand.Text = "";
            txtChassisNumber.Text = "";
            cbStations.SelectedValue = -1;
            cbVehicles.SelectedValue = -1;
            dpInspectionDate.SelectedDate = null;
            txtModel.Text = "";
            txtYear.Text = "";
            txtFuelType.Text = "";
            dataGridInspectionHistory.SelectedItem = null;
        }

        private void RegisterInspection(object sender, RoutedEventArgs e)
        {
           Inspection inspection =new Inspection();
            if (!dpInspectionDate.SelectedDate.HasValue || cbStations.SelectedValue == null || cbVehicles.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DateTime selectedDate = dpInspectionDate.SelectedDate.Value;

            // Lấy ngày hiện tại
            DateTime currentDate = DateTime.Now;

            // Kiểm tra xem ngày chọn có nhỏ hơn ngày hiện tại hay không
            if (selectedDate.Date < currentDate.Date)
            {
                MessageBox.Show("Ngày không thể nhỏ hơn ngày hiện tại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            int cid = int.Parse(cbStations.SelectedValue.ToString());
            int vid = int.Parse(cbVehicles.SelectedValue.ToString());
            if (InspectionDAO.GetInspectionByVehicle(vid))
            {

                MessageBox.Show("Đã tồn tại 1 đăng ký!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            //MessageBox.Show($"{accountId} - {password} - {roleId}");
            inspection.Status = "Chờ xác nhận";
            inspection.ScheduledDate = selectedDate;
            inspection.CenterId = cid;
            inspection.VehicleId = vid;
            InspectionDAO.AddInspection(inspection);
            LoadDataGridInspections();
        }
        void LoadComboboxCenter()
        {

            var center = InspectionCenterDAO.GetInspectionCenters();
            this.cbStations.ItemsSource = center;
            this.cbStations.DisplayMemberPath = "Name";
            this.cbStations.SelectedValuePath = "CenterId";
            this.cbStations.SelectedIndex = -1;
        }
        void LoadComboboxVehicle()
        {

            var vehicle = VehicleDAO.GetVehicleByOwnerIEmail(SessionManager.UserAccount.Email);
            this.cbVehicles.ItemsSource = vehicle;
            this.cbVehicles.DisplayMemberPath = "LicensePlate";
            this.cbVehicles.SelectedValuePath = "VehicleId";
            this.cbVehicles.SelectedIndex = -1;
        }

        private void cbVehicles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (cbVehicles.SelectedItem != null)
            {

                Vehicle vehicle = cbVehicles.SelectedItem as Vehicle;
                txtBrand.Text = vehicle.Brand;
                txtFuelType.Text =vehicle.FuelType;
                txtModel.Text = vehicle.Model;
                txtYear.Text = vehicle.ManufactureYear.ToString();
                txtChassisNumber.Text = vehicle.EngineNumber;
               
            }
        }

        void LoadDataGridInspections()
        {
            UserAccount? acc = SessionManager.UserAccount;
            var inspection = InspectionDAO.GetInspectionByOwnerIEmail(acc.Email);


            this.dataGridInspectionHistory.ItemsSource = inspection;
        }

        private void dgInspection(object sender, SelectionChangedEventArgs e)
        {
            Inspection inps = dataGridInspectionHistory.SelectedItem as Inspection;
            if (inps != null)
            {
                txtid.Text = inps.InspectionId.ToString();
                cbStations.Text = inps.Center.Name;
                cbVehicles.Text = inps.Vehicle.LicensePlate;
                Vehicle vehicle = VehicleDAO.GetVehicleByLicensePlate(inps.Vehicle.LicensePlate);
                txtBrand.Text = vehicle.Brand;
                dpInspectionDate.SelectedDate = inps.ScheduledDate;
                txtModel.Text = vehicle.Model;
                txtYear.Text = vehicle.ManufactureYear.ToString();
                txtChassisNumber.Text = vehicle.EngineNumber;
                txtFuelType.Text = vehicle.FuelType;
            }
        }
        private void btnViewEmissionDetails_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var inspection = button.DataContext as Inspection;

            if (inspection != null)
            {
                var emissionDetailsWindow = new EmissionDetailsWindow(inspection.Vehicle.LicensePlate);
                emissionDetailsWindow.ShowDialog();
            }
        }
    }
}
