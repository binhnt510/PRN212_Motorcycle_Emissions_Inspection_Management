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
    /// Interaction logic for PayFineWindow.xaml
    /// </summary>
    public partial class PayFineWindow : UserControl
    {
        public PayFineWindow()
        {
            InitializeComponent();
            RefreshData(SessionManager.UserAccount.Email);
        }

        private void RefreshData(string email)
        {
            try
            {
                UserAccount? acc = SessionManager.UserAccount;
                var violations = ViolationDAO.GetViolations(acc.Email);
                dataGridViolations.ItemsSource = violations;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void UpdatePayFine(int violationID, string email)
        {
            try
            {
                using (var context = new PVehicleContext())
                {
                    var violation = context.Violations.FirstOrDefault(v => v.ViolationId == violationID);
                    if (violation != null)
                    {
                        violation.Status = "Đã xử lý";
                        violation.PayFine = true;
                        context.SaveChanges();
                    }
                }
                email = SessionManager.UserAccount.Email;
                MessageBox.Show("Nộp phạt thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                RefreshData(email); // Làm mới danh sách sau khi cập nhật
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi nộp phạt: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void PayFine_Click(object sender, RoutedEventArgs e)
        {
            var selectedViolation = dataGridViolations.SelectedItem as Violation;
            if (selectedViolation == null)
            {
                MessageBox.Show("Vui lòng chọn một vi phạm để nộp phạt.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }else if (selectedViolation.PayFine==true)
            {
                MessageBox.Show("Đã nộp phạt","Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string ownerEmail = SessionManager.UserAccount.Email;

            MessageBoxResult result = MessageBox.Show("Xác nhận thanh toán?", "Thanh toán", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                UpdatePayFine(selectedViolation.ViolationId, ownerEmail);
            }
        }
    } 

}
