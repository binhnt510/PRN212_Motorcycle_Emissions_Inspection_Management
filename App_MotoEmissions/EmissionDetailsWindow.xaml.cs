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
using App_MotoEmissions.Models;

namespace App_MotoEmissions
{
    /// <summary>
    /// Interaction logic for EmissionDetailsWindow.xaml
    /// </summary>
    public partial class EmissionDetailsWindow : Window
    {
        public EmissionDetailsWindow(string licensePlate)
        {
            InitializeComponent();
            LoadEmissionDetails(licensePlate);
        }

        private void LoadEmissionDetails(string licensePlate)
        {
            using (var context = new PVehicleContext())
            {
                var emissionDetail = context.Inspections
                    .Where(i => i.Vehicle.LicensePlate == licensePlate && i.Status == "Đã hoàn thành")
                    .Select(i => new
                    {
                        Vehicle = i.Vehicle,
                        EmissionTest = i.EmissionTests.FirstOrDefault()
                    })
                    .FirstOrDefault();

                if (emissionDetail != null)
                {
                    txtLicensePlate.Text = emissionDetail.Vehicle.LicensePlate;
                    txtCoLevel.Text = emissionDetail.EmissionTest?.CoLevel.ToString() + " ppm";
                    txtHcLevel.Text = emissionDetail.EmissionTest?.HcLevel.ToString() + " ppm";
                    txtNoxLevel.Text = emissionDetail.EmissionTest?.NoxLevel.ToString() + " ppm";
                    txtResult.Text = emissionDetail.EmissionTest?.Result;
                    txtTestDate.Text = emissionDetail.EmissionTest?.TestDate.ToString();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy thông tin kiểm tra khí thải.", "Thông Báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
            }
        }
    }
}
