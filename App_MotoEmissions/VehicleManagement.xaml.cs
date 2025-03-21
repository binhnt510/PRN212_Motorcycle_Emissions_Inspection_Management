﻿using System;
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
using App_MotoEmissions.EntityView;
using App_MotoEmissions.Models;

namespace App_MotoEmissions
{
    /// <summary>
    /// Interaction logic for VehicleManagement.xaml
    /// </summary>
    public partial class VehicleManagement : Window
    {
        public VehicleManagement()
        {
            InitializeComponent(); string? userEmail = SessionManager.UserEmail;
            LoadDataGridVehicles();
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void dgAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        void LoadDataGridVehicles()
        {
            UserAccount? acc = SessionManager.UserAccount;
            var vehicles = VehicleDAO.GetVehicleByOwnerIEmail(acc.Email);
            var vehicleViewModels = vehicles.Select((v, index) => new VehicleViewModel
            {
                Number = index + 1,
                Vehicle = v
            }).ToList();

            this.dgVehicles.ItemsSource = vehicleViewModels;
        }
    }
}
