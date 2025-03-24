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
using Microsoft.IdentityModel.Tokens;

namespace App_MotoEmissions
{
    /// <summary>
    /// Interaction logic for VehicleManagementWindow.xaml
    /// </summary>
    public partial class VehicleManagementWindow : UserControl
    {
        public VehicleManagementWindow()
        {
            InitializeComponent(); 
            string? userEmail = SessionManager.UserAccount.Email;
            LoadDataGridVehicles();
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string searchText = txtSearch.Text;
            if (!searchText.IsNullOrEmpty())
            {

                var vehicle = VehicleDAO.GetVehicleByOwnerIEmail(SessionManager.UserAccount.Email)
                .Where(a => a.LicensePlate.ToLower().Contains(searchText.ToLower()));
                this.dgVehicles.ItemsSource = vehicle;

            }
            else
            {
                MessageBox.Show("Vui lòng nhập thông tin tìm kiếm", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

        }
        private void btnDeleteSearch_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = "";
            LoadDataGridVehicles();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            int year;
            if (!int.TryParse(txtYear.Text, out year) || year <= 1860)
            {
                MessageBox.Show("Năm không đúng định dạng hoặc không hợp lệ", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            UserAccount owner = SessionManager.UserAccount;
            string brand = this.txtBrand.Text;
            string engine = this.txtEngine.Text;
            string licensePlate = this.txtLicensePlate.Text;
            string model = this.txtModel.Text;
            string fuel = (cmbFuel.SelectedItem as ComboBoxItem)?.Content.ToString();
            //MessageBox.Show($"{accountId} - {password} - {roleId}");
            if (VehicleDAO.CheckLicensePlateExist(licensePlate))
            {
                MessageBox.Show("Biển số xe đã tồn tại", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;

            }
            if (brand.IsNullOrEmpty()|| engine.IsNullOrEmpty() || licensePlate.IsNullOrEmpty() || model.IsNullOrEmpty() || fuel.IsNullOrEmpty())
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;

            }
            Vehicle vehicle = new Vehicle()
            {
                OwnerId = owner.UserId,
                Brand = brand,
                EngineNumber = engine,
                LicensePlate = licensePlate,
                Model = model,
                FuelType = fuel,
                ManufactureYear = year
            };
            VehicleDAO.AddVehicle(vehicle);
            LoadDataGridVehicles();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            int year;
            if (!int.TryParse(txtYear.Text, out year) || year <= 1860)
            {
                MessageBox.Show("Năm không đúng định dạng hoặc không hợp lệ", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string id = this.txtID.Text;
            string brand = this.txtBrand.Text;
            string engine = this.txtEngine.Text;
            string licensePlate = this.txtLicensePlate.Text;
            string model = this.txtModel.Text;
            string fuel = (cmbFuel.SelectedItem as ComboBoxItem)?.Content.ToString();
            //MessageBox.Show($"{accountId} - {password} - {roleId}");
           
            VehicleDAO.UpdateVehicle(int.Parse(id), licensePlate, engine, brand, model, fuel, year);
            LoadDataGridVehicles();



        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtLicensePlate.Text = "";
            txtBrand.Text = "";
            txtEngine.Text = "";
            cmbFuel.SelectedValue = -1;
            txtModel.Text = "";
            txtYear.Text = "";
            txtID.Text = "";
            dgVehicles.SelectedItem = null;
            txtLicensePlate.IsReadOnly = false;

        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            int year;
            if (!int.TryParse(txtYear.Text, out year) || year <= 1860)
            {
                MessageBox.Show("Năm không đúng định dạng hoặc không hợp lệ", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string id = this.txtID.Text;
            string brand = this.txtBrand.Text;
            string engine = this.txtEngine.Text;
            string licensePlate = this.txtLicensePlate.Text;
            string model = this.txtModel.Text;
            string fuel = (cmbFuel.SelectedItem as ComboBoxItem)?.Content.ToString();
            //MessageBox.Show($"{accountId} - {password} - {roleId}");
            if (MessageBox.Show("Bạn có chắc chắn muốn xóa phương tiện?",
                    "Xác nhận",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                VehicleDAO.DeleteVehicle(int.Parse(id));
                LoadDataGridVehicles();
            }

            

        }


        private void dgVehicles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Vehicle vehicle = dgVehicles.SelectedItem as Vehicle;
            if (vehicle != null)
            {
                txtLicensePlate.Text = vehicle.LicensePlate;
                txtBrand.Text = vehicle.Brand;
                txtEngine.Text = vehicle.EngineNumber;
                cmbFuel.SelectedValue= vehicle.FuelType;
                txtModel.Text = vehicle.Model;
                txtYear.Text = vehicle.ManufactureYear.ToString();
                txtID.Text =vehicle.VehicleId.ToString();
            }
            //MessageBox.Show($"{account.AccountId} - {account.Password} - {account.RoleId}");
        }
        void LoadDataGridVehicles()
        {
            UserAccount? acc = SessionManager.UserAccount;
            var vehicles = VehicleDAO.GetVehicleByOwnerIEmail(acc.Email);
            

            this.dgVehicles.ItemsSource = vehicles;
        }

    }
}
