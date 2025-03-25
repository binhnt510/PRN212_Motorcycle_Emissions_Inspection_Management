using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using App_MotoEmissions.DAO;
using LiveCharts.Wpf;
using LiveCharts;
using LiveCharts.Defaults;

namespace App_MotoEmissions
{
    public partial class ReportAndStatisticsWindow : UserControl
    {
        public ReportAndStatisticsWindow()
        {
            InitializeComponent();
            LoadAreas();
        }

        private void LoadAreas()
        {
            cbAreaFilter.ItemsSource = EmissionTestDAO.GetAllAreas();
        }

        private void btnGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            DateTime? startDate = dpStartDate.SelectedDate;
            DateTime? endDate = dpEndDate.SelectedDate;
            string selectedArea = cbAreaFilter.SelectedItem as string;

            if (startDate == null || endDate == null)
            {
                MessageBox.Show("Vui lòng chọn khoảng thời gian.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var reportData = EmissionTestDAO.GetEmissionStatistics(startDate.Value, endDate.Value, selectedArea);

            if (reportData.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu trong khoảng thời gian này.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                dgStatistics.ItemsSource = null;
                chartContainer.Children.Clear();
                return;
            }

            dgStatistics.ItemsSource = reportData;
            GenerateChart(reportData);
        }

        private void GenerateChart(List<EmissionReport> reportData)
        {
            int passCount = reportData.Sum(r => r.Passed);
            int failCount = reportData.Sum(r => r.Failed);

            if (passCount == 0 && failCount == 0)
            {
                chartContainer.Children.Clear();
                return;
            }

            var pieChart = new PieChart
            {
                Series = new SeriesCollection
                {
                    new PieSeries
                    {
                        Title = "Đạt",
                        Values = new ChartValues<ObservableValue> { new ObservableValue(passCount) },
                        DataLabels = true
                    },
                    new PieSeries
                    {
                        Title = "Không đạt",
                        Values = new ChartValues<ObservableValue> { new ObservableValue(failCount) },
                        DataLabels = true
                    }
                }
            };

            chartContainer.Children.Clear();
            chartContainer.Children.Add(pieChart);
        }
    }
}
