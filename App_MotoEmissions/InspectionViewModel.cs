using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Windows;
using App_MotoEmissions.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using static Azure.Core.HttpHeader;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using App_MotoEmissions.DAO;

public class InspectionViewModel
{
    private readonly string _connectionString;

    // Constructor với Dependency Injection
    public InspectionViewModel(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Default");
    }

    // Constructor cho mục đích test
    public InspectionViewModel(string connectionString)
    {
        _connectionString = connectionString;
    }
    public UserAccount GetUserByVehicle(int vehicleId)
    {
        using (var db = new PVehicleContext()) // Đảm bảo dùng đúng DbContext của bạn
        {
            return db.UserAccounts
                     .FirstOrDefault(u => db.Vehicles.Any(v => v.VehicleId == vehicleId && v.OwnerId == u.UserId));
        }
    }

    // Phương thức cập nhật kết quả kiểm định
    public bool SubmitInspectionResult(int inspectionId, double co, double hc, double nox, string result, string notes)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Lấy thông tin xe
                        string vehicleQuery = @"
                        SELECT v.OwnerID, v.LicensePlate, u.Email 
                        FROM Inspection i
                        JOIN Vehicle v ON i.VehicleID = v.VehicleID
                        JOIN UserAccount u ON v.OwnerID = u.UserID
                        WHERE i.InspectionID = @InspectionID";

                        UserAccount owner = null;
                        string licensePlate = "";

                        using (SqlCommand vehicleCmd = new SqlCommand(vehicleQuery, connection, transaction))
                        {
                            vehicleCmd.Parameters.AddWithValue("@InspectionID", inspectionId);

                            using (SqlDataReader reader = vehicleCmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    owner = new UserAccount
                                    {
                                        UserId = reader.GetInt32(reader.GetOrdinal("OwnerID")),
                                        Email = reader.GetString(reader.GetOrdinal("Email"))
                                    };
                                    licensePlate = reader.GetString(reader.GetOrdinal("LicensePlate"));
                                }
                            }
                        }

                        // Cập nhật trạng thái kiểm định
                        string updateQuery = "UPDATE Inspection SET Status = 'Đã xác nhận' WHERE InspectionID = @InspectionID;";
                        using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection, transaction))
                        {
                            updateCommand.Parameters.AddWithValue("@InspectionID", inspectionId);
                            updateCommand.ExecuteNonQuery();
                        }

                        // Thêm kết quả kiểm định vào bảng EmissionTest
                        string insertQuery = @"
                    INSERT INTO EmissionTest (InspectionID, CO_Level, HC_Level, NOx_Level, Result, TestDate) 
                    VALUES (@InspectionID, @CO_Level, @HC_Level, @NOx_Level, @Result, SYSDATETIME());";

                        using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection, transaction))
                        {
                            insertCommand.Parameters.AddWithValue("@InspectionID", inspectionId);
                            insertCommand.Parameters.AddWithValue("@CO_Level", co);
                            insertCommand.Parameters.AddWithValue("@HC_Level", hc);
                            insertCommand.Parameters.AddWithValue("@NOx_Level", nox);
                            insertCommand.Parameters.AddWithValue("@Result", result);

                            insertCommand.ExecuteNonQuery();
                        }

                        // Tạo thông báo cho chủ xe
                        if (owner != null)
                        {
                            string notificationMessage = $"Kết quả kiểm định xe {licensePlate}: {result}. " +
                                $"Chi tiết: CO: {co}%, HC: {hc} ppm, NOx: {nox} ppm. " +
                                $"Ghi chú: {notes}";

                            // Sử dụng NotificationDAO để tạo thông báo
                            NotificationDAO.CreateNotification(owner, notificationMessage);
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi kết nối CSDL: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }

    // Lấy danh sách kiểm định
    public List<InspectionData> GetInspectionList(string statusFilter = null, DateTime? dateFilter = null)
    {
        List<InspectionData> inspections = new List<InspectionData>();

        try
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"
                    SELECT i.InspectionID, v.LicensePlate, u.FullName AS OwnerName,
                           v.Brand, v.Model, i.ScheduledDate, i.Status
                    FROM Inspection i
                    JOIN Vehicle v ON i.VehicleID = v.VehicleID
                    JOIN UserAccount u ON v.OwnerID = u.UserID
                    WHERE 1=1";

                var parameters = new List<SqlParameter>();

                if (!string.IsNullOrEmpty(statusFilter))
                {
                    query += " AND i.Status = @Status";
                    parameters.Add(new SqlParameter("@Status", statusFilter));
                }

                if (dateFilter.HasValue)
                {
                    query += " AND CAST(i.ScheduledDate AS DATE) = @ScheduledDate";
                    parameters.Add(new SqlParameter("@ScheduledDate", dateFilter.Value.Date));
                }

                query += " ORDER BY i.ScheduledDate";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddRange(parameters.ToArray());
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            inspections.Add(new InspectionData
                            {
                                InspectionID = reader.GetInt32("InspectionID"),
                                LicensePlate = reader.GetString("LicensePlate"),
                                OwnerName = reader.GetString("OwnerName"),
                                Brand = reader.GetString("Brand"),
                                Model = reader.GetString("Model"),
                                ScheduledDate = reader.GetDateTime("ScheduledDate"),
                                Status = reader.GetString("Status")
                            });
                        }
                    }
                }
            }
        }
        catch (SqlException ex)
        {
            MessageBox.Show($"Lỗi SQL: {ex.Message}\nMã lỗi: {ex.Number}", "Lỗi CSDL", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi hệ thống: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        return inspections;
    }

    // Lấy dữ liệu kiểm tra khí thải theo InspectionID
    public EmissionTestData GetEmissionTestByInspectionId(int inspectionId)
    {
        EmissionTestData emissionTest = null;

        try
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
                SELECT CO_Level, HC_Level, NOx_Level, Result, TestDate 
                FROM EmissionTest 
                WHERE InspectionID = @InspectionID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@InspectionID", inspectionId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())  // Nếu tìm thấy dữ liệu
                        {
                            emissionTest = new EmissionTestData
                            {
                                CO = reader.GetDouble(reader.GetOrdinal("CO_Level")),
                                HC = reader.GetDouble(reader.GetOrdinal("HC_Level")),
                                NOx = reader.GetDouble(reader.GetOrdinal("NOx_Level")),
                                Notes = reader["Result"]?.ToString(),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("TestDate"))
                            };
                        }
                    }
                }
            }
        }
        catch (SqlException ex)
        {
            MessageBox.Show("Lỗi SQL: " + ex.Message, "Lỗi CSDL", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Lỗi hệ thống: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        return emissionTest;
    }

    public bool SaveEmissionTest(int inspectionId, double coLevel, double hcLevel, double noxLevel, string result)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "INSERT INTO EmissionTest (InspectionID, CoLevel, HcLevel, NoxLevel, Result, TestDate) VALUES (@InspectionID, @CoLevel, @HcLevel, @NoxLevel, @Result, @TestDate)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@InspectionID", inspectionId);
                    cmd.Parameters.AddWithValue("@CoLevel", coLevel);
                    cmd.Parameters.AddWithValue("@HcLevel", hcLevel);
                    cmd.Parameters.AddWithValue("@NoxLevel", noxLevel);
                    cmd.Parameters.AddWithValue("@Result", result);
                    cmd.Parameters.AddWithValue("@TestDate", DateTime.Now);

                    cmd.ExecuteNonQuery();
                }
            }

            return true;
        }
        catch (SqlException ex)
        {
            MessageBox.Show("Lỗi khi lưu kiểm định: " + ex.Message);
            return false;
        }
    }




    // Lớp dữ liệu phụ trợ
    public class InspectionData
    {
        public int InspectionID { get; set; }
        public string LicensePlate { get; set; }
        public string OwnerName { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public DateTime ScheduledDate { get; set; }
        public string Status { get; set; }
    }

    public class EmissionTestData
    {
        public double CO { get; set; }
        public double HC { get; set; }
        public double NOx { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
