using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Data;

namespace EmissionInspectionSystem
{
    public partial class Window1 : Window
    {
        // Connection string - replace with your actual connection string
        private string connectionString = "Data Source=VIVOBOOK_PRO_15;Initial Catalog=P_Vehicle;Integrated Security=True";
        private CollectionViewSource inspectionViewSource;
        private CollectionViewSource pendingInspectionViewSource;
        private int currentPage = 1;
        private int pageSize = 10;
        private int totalPages = 1;

        // Selected inspection for emission test
        private InspectionViewModel selectedInspection = null;

        public Window1()
        {
            InitializeComponent();
            LoadInspections();
            LoadPendingInspections();
        }

        #region Inspection Schedule Management

        private void LoadInspections()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT i.InspectionID, v.LicensePlate, u.FullName AS OwnerName,
                               v.Brand, v.Model, i.ScheduledDate, i.Status
                        FROM Inspection i
                        INNER JOIN Vehicle v ON i.VehicleID = v.VehicleID
                        INNER JOIN UserAccount u ON v.OwnerID = u.UserID
                        ORDER BY i.ScheduledDate DESC
                        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Offset", (currentPage - 1) * pageSize);
                    command.Parameters.AddWithValue("@PageSize", pageSize);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Count total records for pagination
                    string countQuery = "SELECT COUNT(*) FROM Inspection";
                    SqlCommand countCommand = new SqlCommand(countQuery, connection);
                    int totalRecords = (int)countCommand.ExecuteScalar();
                    totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                    // Update UI
                    var inspectionsGrid = (DataGrid)this.FindName("inspectionsGrid");
                    inspectionsGrid.ItemsSource = dataTable.DefaultView;
                    UpdatePaginationDisplay();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading inspections: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var statusComboBox = (ComboBox)this.FindName("statusComboBox");
            var datePicker = (DatePicker)this.FindName("datePicker");

            string selectedStatus = statusComboBox.SelectedItem != null ?
                ((ComboBoxItem)statusComboBox.SelectedItem).Content.ToString() : "Tất cả";

            DateTime? selectedDate = datePicker.SelectedDate;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT i.InspectionID, v.LicensePlate, u.FullName AS OwnerName,
                               v.Brand, v.Model, i.ScheduledDate, i.Status
                        FROM Inspection i
                        INNER JOIN Vehicle v ON i.VehicleID = v.VehicleID
                        INNER JOIN UserAccount u ON v.OwnerID = u.UserID
                        WHERE 1=1 ";

                    // Add filters
                    if (selectedStatus != "Tất cả")
                    {
                        query += " AND i.Status = @Status";
                    }

                    if (selectedDate.HasValue)
                    {
                        query += " AND CAST(i.ScheduledDate AS DATE) = @ScheduledDate";
                    }

                    query += @" ORDER BY i.ScheduledDate DESC
                        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                    SqlCommand command = new SqlCommand(query, connection);

                    if (selectedStatus != "Tất cả")
                    {
                        command.Parameters.AddWithValue("@Status", selectedStatus);
                    }

                    if (selectedDate.HasValue)
                    {
                        command.Parameters.AddWithValue("@ScheduledDate", selectedDate.Value.Date);
                    }

                    command.Parameters.AddWithValue("@Offset", (currentPage - 1) * pageSize);
                    command.Parameters.AddWithValue("@PageSize", pageSize);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Count total filtered records for pagination
                    string countQuery = @"
                        SELECT COUNT(*) 
                        FROM Inspection i
                        WHERE 1=1 ";

                    if (selectedStatus != "Tất cả")
                    {
                        countQuery += " AND i.Status = @Status";
                    }

                    if (selectedDate.HasValue)
                    {
                        countQuery += " AND CAST(i.ScheduledDate AS DATE) = @ScheduledDate";
                    }

                    SqlCommand countCommand = new SqlCommand(countQuery, connection);

                    if (selectedStatus != "Tất cả")
                    {
                        countCommand.Parameters.AddWithValue("@Status", selectedStatus);
                    }

                    if (selectedDate.HasValue)
                    {
                        countCommand.Parameters.AddWithValue("@ScheduledDate", selectedDate.Value.Date);
                    }

                    int totalRecords = (int)countCommand.ExecuteScalar();
                    totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                    // Update UI
                    var inspectionsGrid = (DataGrid)this.FindName("inspectionsGrid");
                    inspectionsGrid.ItemsSource = dataTable.DefaultView;
                    UpdatePaginationDisplay();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching inspections: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            // Reset filters
            var statusComboBox = (ComboBox)this.FindName("statusComboBox");
            var datePicker = (DatePicker)this.FindName("datePicker");

            statusComboBox.SelectedIndex = 0;
            datePicker.SelectedDate = null;

            // Reset pagination
            currentPage = 1;

            // Reload data
            LoadInspections();
        }

        private void PreviousPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                LoadInspections();
            }
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                LoadInspections();
            }
        }

        private void UpdatePaginationDisplay()
        {
            var pageInfoText = (TextBlock)this.FindName("pageInfoText");
            pageInfoText.Text = $"Trang {currentPage}/{totalPages}";
        }

        private void ViewDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            DataRowView rowView = (DataRowView)button.DataContext;
            int inspectionId = Convert.ToInt32(rowView["InspectionID"]);

            // Open inspection details window
            InspectionDetailsWindow detailsWindow = new InspectionDetailsWindow(inspectionId);
            detailsWindow.ShowDialog();
        }

        private void StartInspectionButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            DataRowView rowView = (DataRowView)button.DataContext;
            int inspectionId = Convert.ToInt32(rowView["InspectionID"]);
            string status = rowView["Status"].ToString();

            if (status == "Chờ kiểm tra")
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string updateQuery = "UPDATE Inspection SET Status = 'Đang kiểm tra' WHERE InspectionID = @InspectionID";
                        SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                        updateCommand.Parameters.AddWithValue("@InspectionID", inspectionId);
                        updateCommand.ExecuteNonQuery();

                        MessageBox.Show("Trạng thái kiểm tra đã được cập nhật thành 'Đang kiểm tra'", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Refresh the list
                        LoadInspections();
                        LoadPendingInspections();

                        // Switch to emission test tab
                        var tabControl = (TabControl)this.FindName("tabControl");
                        tabControl.SelectedIndex = 1;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating inspection status: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Chỉ có thể bắt đầu kiểm tra khi trạng thái là 'Chờ kiểm tra'", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        #endregion

        #region Emission Testing

        private void LoadPendingInspections()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT i.InspectionID, v.LicensePlate, u.FullName AS OwnerName, i.ScheduledDate
                        FROM Inspection i
                        INNER JOIN Vehicle v ON i.VehicleID = v.VehicleID
                        INNER JOIN UserAccount u ON v.OwnerID = u.UserID
                        WHERE i.Status = 'Đang kiểm tra'
                        ORDER BY i.ScheduledDate ASC";

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    var pendingInspectionsGrid = (DataGrid)this.FindName("pendingInspectionsGrid");
                    pendingInspectionsGrid.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading pending inspections: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SelectVehicleButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            DataRowView rowView = (DataRowView)button.DataContext;
            int inspectionId = Convert.ToInt32(rowView["InspectionID"]);
            string licensePlate = rowView["LicensePlate"].ToString();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT v.LicensePlate, v.Brand, v.Model, v.ManufactureYear,
                               i.InspectionID, i.VehicleID
                        FROM Vehicle v
                        INNER JOIN Inspection i ON v.VehicleID = i.VehicleID
                        WHERE i.InspectionID = @InspectionID";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@InspectionID", inspectionId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Display vehicle information in the form
                            var licenseTextBlock = (TextBlock)this.FindName("licensePlateDisplay");
                            var brandTextBlock = (TextBlock)this.FindName("brandDisplay");
                            var modelTextBlock = (TextBlock)this.FindName("modelDisplay");
                            var yearTextBlock = (TextBlock)this.FindName("yearDisplay");

                            licenseTextBlock.Text = reader["LicensePlate"].ToString();
                            brandTextBlock.Text = reader["Brand"].ToString();
                            modelTextBlock.Text = reader["Model"].ToString();
                            yearTextBlock.Text = reader["ManufactureYear"].ToString();

                            // Store selected inspection for saving results
                            selectedInspection = new InspectionViewModel
                            {
                                InspectionID = inspectionId,
                                VehicleID = Convert.ToInt32(reader["VehicleID"])
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading vehicle details: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveResultButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedInspection == null)
            {
                MessageBox.Show("Vui lòng chọn một xe để kiểm tra trước", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Get input values
            var coLevelTextBox = (TextBox)this.FindName("coLevelTextBox");
            var hcLevelTextBox = (TextBox)this.FindName("hcLevelTextBox");
            var noxLevelTextBox = (TextBox)this.FindName("noxLevelTextBox");
            var passRadioButton = (RadioButton)this.FindName("passRadioButton");
            var notesTextBox = (TextBox)this.FindName("notesTextBox");

            // Validate input
            if (string.IsNullOrWhiteSpace(coLevelTextBox.Text) ||
                string.IsNullOrWhiteSpace(hcLevelTextBox.Text) ||
                string.IsNullOrWhiteSpace(noxLevelTextBox.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông số khí thải", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Parse input values
            float coLevel, hcLevel, noxLevel;

            if (!float.TryParse(coLevelTextBox.Text, out coLevel) ||
                !float.TryParse(hcLevelTextBox.Text, out hcLevel) ||
                !float.TryParse(noxLevelTextBox.Text, out noxLevel))
            {
                MessageBox.Show("Thông số khí thải phải là số", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Determine result
            string result = passRadioButton.IsChecked == true ? "Đạt" : "Không đạt";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        // Insert emission test results
                        string insertQuery = @"
                            INSERT INTO EmissionTest (InspectionID, CO_Level, HC_Level, NOx_Level, Result, TestDate)
                            VALUES (@InspectionID, @CO_Level, @HC_Level, @NOx_Level, @Result, GETDATE())";

                        SqlCommand insertCommand = new SqlCommand(insertQuery, connection, transaction);
                        insertCommand.Parameters.AddWithValue("@InspectionID", selectedInspection.InspectionID);
                        insertCommand.Parameters.AddWithValue("@CO_Level", coLevel);
                        insertCommand.Parameters.AddWithValue("@HC_Level", hcLevel);
                        insertCommand.Parameters.AddWithValue("@NOx_Level", noxLevel);
                        insertCommand.Parameters.AddWithValue("@Result", result);
                        insertCommand.ExecuteNonQuery();

                        // Update inspection status
                        string updateQuery = "UPDATE Inspection SET Status = 'Hoàn thành' WHERE InspectionID = @InspectionID";
                        SqlCommand updateCommand = new SqlCommand(updateQuery, connection, transaction);
                        updateCommand.Parameters.AddWithValue("@InspectionID", selectedInspection.InspectionID);
                        updateCommand.ExecuteNonQuery();

                        // Create notification for vehicle owner
                        string notificationQuery = @"
                            INSERT INTO [Notification] (UserID, Message, SentDate, IsRead)
                            SELECT v.OwnerID, 
                                   CASE WHEN @Result = 'Đạt' 
                                        THEN N'Xe của bạn đã kiểm tra khí thải và đạt tiêu chuẩn.'
                                        ELSE N'Xe của bạn đã kiểm tra khí thải và không đạt tiêu chuẩn. Vui lòng liên hệ trung tâm để biết thêm chi tiết.'
                                   END,
                                   GETDATE(), 0
                            FROM Vehicle v
                            INNER JOIN Inspection i ON v.VehicleID = i.VehicleID
                            WHERE i.InspectionID = @InspectionID";

                        SqlCommand notificationCommand = new SqlCommand(notificationQuery, connection, transaction);
                        notificationCommand.Parameters.AddWithValue("@InspectionID", selectedInspection.InspectionID);
                        notificationCommand.Parameters.AddWithValue("@Result", result);
                        notificationCommand.ExecuteNonQuery();

                        transaction.Commit();
                        MessageBox.Show("Đã lưu kết quả kiểm định thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Clear form
                        ClearEmissionTestForm();

                        // Refresh data
                        LoadInspections();
                        LoadPendingInspections();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving emission test results: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ClearEmissionTestForm();
        }

        private void ClearEmissionTestForm()
        {
            // Clear displayed vehicle information
            var licenseTextBlock = (TextBlock)this.FindName("licensePlateDisplay");
            var brandTextBlock = (TextBlock)this.FindName("brandDisplay");
            var modelTextBlock = (TextBlock)this.FindName("modelDisplay");
            var yearTextBlock = (TextBlock)this.FindName("yearDisplay");

            licenseTextBlock.Text = "";
            brandTextBlock.Text = "";
            modelTextBlock.Text = "";
            yearTextBlock.Text = "";

            // Clear emission test form
            var coLevelTextBox = (TextBox)this.FindName("coLevelTextBox");
            var hcLevelTextBox = (TextBox)this.FindName("hcLevelTextBox");
            var noxLevelTextBox = (TextBox)this.FindName("noxLevelTextBox");
            var passRadioButton = (RadioButton)this.FindName("passRadioButton");
            var notesTextBox = (TextBox)this.FindName("notesTextBox");

            coLevelTextBox.Text = "";
            hcLevelTextBox.Text = "";
            noxLevelTextBox.Text = "";
            passRadioButton.IsChecked = true;
            notesTextBox.Text = "";

            // Clear selected inspection
            selectedInspection = null;
        }

        #endregion
    }

    public class InspectionViewModel
    {
        public int InspectionID { get; set; }
        public int VehicleID { get; set; }
        public string LicensePlate { get; set; }
        public string OwnerName { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public DateTime ScheduledDate { get; set; }
        public string Status { get; set; }
    }

    // Additional window for inspection details
    public class InspectionDetailsWindow : Window
    {
        private int inspectionId;
        private string connectionString = "Data Source=YourServer;Initial Catalog=EmissionInspectionDB;Integrated Security=True";

        public InspectionDetailsWindow(int inspectionId)
        {
            this.inspectionId = inspectionId;
            InitializeComponent();
            LoadInspectionDetails();
        }

        private void InitializeComponent()
        {
            this.Title = "Chi tiết kiểm định";
            this.Width = 500;
            this.Height = 600;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            Grid grid = new Grid();
            grid.Margin = new Thickness(20);

            // Set up grid rows
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Header
            TextBlock headerText = new TextBlock
            {
                Text = "Chi tiết kiểm định",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 20)
            };
            grid.Children.Add(headerText);
            Grid.SetRow(headerText, 0);

            // Content
            StackPanel contentPanel = new StackPanel();
            grid.Children.Add(contentPanel);
            Grid.SetRow(contentPanel, 1);

            // Add content here in LoadInspectionDetails

            // Footer
            Button closeButton = new Button
            {
                Content = "Đóng",
                Width = 100,
                Height = 30,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 20, 0, 0)
            };
            closeButton.Click += (sender, e) => this.Close();
            grid.Children.Add(closeButton);
            Grid.SetRow(closeButton, 2);

            this.Content = grid;
        }

        private void LoadInspectionDetails()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT i.InspectionID, i.ScheduledDate, i.Status,
                               v.LicensePlate, v.Brand, v.Model, v.ManufactureYear,
                               u.FullName AS OwnerName, u.PhoneNumber AS OwnerPhone,
                               c.Name AS CenterName, c.Address AS CenterAddress,
                               e.CO_Level, e.HC_Level, e.NOx_Level, e.Result, e.TestDate
                        FROM Inspection i
                        INNER JOIN Vehicle v ON i.VehicleID = v.VehicleID
                        INNER JOIN UserAccount u ON v.OwnerID = u.UserID
                        INNER JOIN InspectionCenter c ON i.CenterID = c.CenterID
                        LEFT JOIN EmissionTest e ON i.InspectionID = e.InspectionID
                        WHERE i.InspectionID = @InspectionID";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@InspectionID", inspectionId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Get content panel
                            StackPanel contentPanel = (StackPanel)((Grid)this.Content).Children[1];
                            contentPanel.Children.Clear();

                            // Vehicle information
                            AddGroupBoxToPanel(contentPanel, "Thông tin xe", new List<KeyValuePair<string, string>>
                            {
                                new KeyValuePair<string, string>("Biển số xe", reader["LicensePlate"].ToString()),
                                new KeyValuePair<string, string>("Hãng xe", reader["Brand"].ToString()),
                                new KeyValuePair<string, string>("Mẫu xe", reader["Model"].ToString()),
                                new KeyValuePair<string, string>("Năm sản xuất", reader["ManufactureYear"].ToString())
                            });

                            // Owner information
                            AddGroupBoxToPanel(contentPanel, "Thông tin chủ xe", new List<KeyValuePair<string, string>>
                            {
                                new KeyValuePair<string, string>("Tên chủ xe", reader["OwnerName"].ToString()),
                                new KeyValuePair<string, string>("Số điện thoại", reader["OwnerPhone"].ToString())
                            });

                            // Inspection information
                            AddGroupBoxToPanel(contentPanel, "Thông tin kiểm định", new List<KeyValuePair<string, string>>
                            {
                                new KeyValuePair<string, string>("Mã kiểm định", reader["InspectionID"].ToString()),
                                new KeyValuePair<string, string>("Trung tâm kiểm định", reader["CenterName"].ToString()),
                                new KeyValuePair<string, string>("Thời gian hẹn", ((DateTime)reader["ScheduledDate"]).ToString("dd/MM/yyyy HH:mm")),
                                new KeyValuePair<string, string>("Trạng thái", reader["Status"].ToString())
                            });

                            // Test results if available
                            if (!reader.IsDBNull(reader.GetOrdinal("TestDate")))
                            {
                                AddGroupBoxToPanel(contentPanel, "Kết quả kiểm tra khí thải", new List<KeyValuePair<string, string>>
                                {
                                    new KeyValuePair<string, string>("Ngày kiểm tra", ((DateTime)reader["TestDate"]).ToString("dd/MM/yyyy HH:mm")),
                                    new KeyValuePair<string, string>("CO (%)", reader["CO_Level"].ToString()),
                                    new KeyValuePair<string, string>("HC (ppm)", reader["HC_Level"].ToString()),
                                    new KeyValuePair<string, string>("NOx (ppm)", reader["NOx_Level"].ToString()),
                                    new KeyValuePair<string, string>("Kết quả", reader["Result"].ToString())
                                });
                            }
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy thông tin kiểm định", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading inspection details: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddGroupBoxToPanel(StackPanel panel, string header, List<KeyValuePair<string, string>> items)
        {
            GroupBox groupBox = new GroupBox
            {
                Header = header,
                Margin = new Thickness(0, 0, 0, 10)
            };

            Grid grid = new Grid
            {
                Margin = new Thickness(10)
            };

            // Set up grid columns
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Add rows
            for (int i = 0; i < items.Count; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            // Add items
            for (int i = 0; i < items.Count; i++)
            {
                TextBlock labelBlock = new TextBlock
                {
                    Text = items[i].Key + ":",
                    Margin = new Thickness(0, 5, 10, 5)
                };

                TextBlock valueBlock = new TextBlock
                {
                    Text = items[i].Value,
                    Margin = new Thickness(0, 5, 0, 5),
                    FontWeight = FontWeights.SemiBold
                };

                grid.Children.Add(labelBlock);
                Grid.SetRow(labelBlock, i);
                Grid.SetColumn(labelBlock, 0);

                grid.Children.Add(valueBlock);
                Grid.SetRow(valueBlock, i);
                Grid.SetColumn(valueBlock, 1);
            }

            groupBox.Content = grid;
            panel.Children.Add(groupBox);
        }
    }
}