using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for InspectionCenterWindow.xaml
    /// </summary>
    public partial class InspectionCenterWindow : UserControl
    {
        public ObservableCollection<InspectionCenter> InspectionCenters { get; set; }
        private ObservableCollection<InspectionCenter> _allInspectionCenters;
        private ObservableCollection<InspectionCenter> _allCenters; // Danh sách gốc
        public ObservableCollection<InspectionCenter> PagedInspectionCenters { get; set; }
        private int _pageSize = 10; // Số mục mỗi trang
        private int _currentPage = 1;
        private int _totalPages;
        private InspectionCenter _selectedCenter;

        public InspectionCenterWindow()
        {
            InitializeComponent();
            LoadInspectionCenters();
            DataContext = this; // Gán DataContext để binding
        }


        private void LoadInspectionCenters()
        {
            _allCenters = new ObservableCollection<InspectionCenter>(InspectionCenterDAO.GetInspectionCenters());
            _totalPages = (int)Math.Ceiling((double)_allCenters.Count / _pageSize);
            _currentPage = 1;
            UpdatePagedData();
        }



        private void UpdatePagedData()
        {
            if (_allCenters == null || _allCenters.Count == 0)
            {
                PagedInspectionCenters = new ObservableCollection<InspectionCenter>();
                lblPageInfo.Content = "Trang 0/0";
                return;
            }

            _totalPages = (int)Math.Ceiling((double)_allCenters.Count / _pageSize);
            _currentPage = Math.Max(1, Math.Min(_currentPage, _totalPages));

            var pagedData = _allCenters.Skip((_currentPage - 1) * _pageSize).Take(_pageSize).ToList();
            PagedInspectionCenters = new ObservableCollection<InspectionCenter>(pagedData);

            dgInspectionCenters.ItemsSource = PagedInspectionCenters;
            lblPageInfo.Content = $"Trang {_currentPage}/{_totalPages}";
        }


        private void btnFirstPage_Click(object sender, RoutedEventArgs e)
        {
            _currentPage = 1;
            UpdatePagedData();
        }

        private void btnPreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                UpdatePagedData();
            }
        }

        private void btnNextPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage < _totalPages)
            {
                _currentPage++;
                UpdatePagedData();
            }
        }

        private void btnLastPage_Click(object sender, RoutedEventArgs e)
        {
            _currentPage = _totalPages;
            UpdatePagedData();
        }

        private void dgInspectionCenters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgInspectionCenters.SelectedItem is InspectionCenter selectedCenter)
            {
                _selectedCenter = selectedCenter;
                txtName.Text = selectedCenter.Name;
                txtAddress.Text = selectedCenter.Address;
                txtPhoneNumber.Text = selectedCenter.PhoneNumber;
                txtEmail.Text = selectedCenter.Email;
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtAddress.Text) ||
                string.IsNullOrWhiteSpace(txtPhoneNumber.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            // Kiểm tra số điện thoại (chỉ chấp nhận 10 chữ số)
            if (!Regex.IsMatch(txtPhoneNumber.Text, @"^\d{10}$"))
            {
                MessageBox.Show("Số điện thoại phải có đúng 10 chữ số!");
                return;
            }

            // Kiểm tra email hợp lệ
            if (!Regex.IsMatch(txtEmail.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Email không hợp lệ!");
                return;
            }

            // Kiểm tra trùng lặp
            bool isDuplicate = _allCenters.Any(c =>
                c.Name.Equals(txtName.Text, StringComparison.OrdinalIgnoreCase) &&
                c.Address.Equals(txtAddress.Text, StringComparison.OrdinalIgnoreCase) &&
                c.PhoneNumber.Equals(txtPhoneNumber.Text, StringComparison.OrdinalIgnoreCase) &&
                c.Email.Equals(txtEmail.Text, StringComparison.OrdinalIgnoreCase));

            if (isDuplicate)
            {
                MessageBox.Show("Trung tâm này đã tồn tại với cùng thông tin. Không thể thêm!");
                return;
            }

            var newCenter = new InspectionCenter
            {
                Name = txtName.Text,
                Address = txtAddress.Text,
                PhoneNumber = txtPhoneNumber.Text,
                Email = txtEmail.Text
            };

            if (InspectionCenterDAO.AddInspectionCenter(newCenter))
            {
                // Kiểm tra xem danh sách đã khởi tạo chưa
                if (_allCenters == null)
                {
                    _allCenters = new ObservableCollection<InspectionCenter>();
                }

                // Thêm vào danh sách gốc
                _allCenters.Add(newCenter);

                // Cập nhật tổng số trang
                _totalPages = (int)Math.Ceiling((double)_allCenters.Count / _pageSize);

                // Chuyển đến trang cuối để hiển thị dữ liệu mới
                _currentPage = _totalPages;

                // Cập nhật DataGrid
                UpdatePagedData();

                MessageBox.Show("Thêm thành công!");
            }
            else
            {
                MessageBox.Show("Lỗi khi thêm!");
            }
        }



        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCenter == null)
            {
                MessageBox.Show("Chọn một trung tâm để sửa!");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtAddress.Text) ||
                string.IsNullOrWhiteSpace(txtPhoneNumber.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            // Kiểm tra số điện thoại (chỉ chấp nhận 10 chữ số)
            if (!Regex.IsMatch(txtPhoneNumber.Text, @"^\d{10}$"))
            {
                MessageBox.Show("Số điện thoại phải có đúng 10 chữ số!");
                return;
            }

            // Kiểm tra email hợp lệ
            if (!Regex.IsMatch(txtEmail.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Email không hợp lệ!");
                return;
            }

            // Kiểm tra trùng lặp (không tính chính bản thân nó)
            bool isDuplicate = _allCenters.Any(c =>
                c.CenterId != _selectedCenter.CenterId &&
                c.Name.Equals(txtName.Text, StringComparison.OrdinalIgnoreCase) &&
                c.Address.Equals(txtAddress.Text, StringComparison.OrdinalIgnoreCase) &&
                c.PhoneNumber.Equals(txtPhoneNumber.Text, StringComparison.OrdinalIgnoreCase) &&
                c.Email.Equals(txtEmail.Text, StringComparison.OrdinalIgnoreCase));

            if (isDuplicate)
            {
                MessageBox.Show("Thông tin trùng với một trung tâm khác. Không thể cập nhật!");
                return;
            }

            // Cập nhật dữ liệu
            _selectedCenter.Name = txtName.Text;
            _selectedCenter.Address = txtAddress.Text;
            _selectedCenter.PhoneNumber = txtPhoneNumber.Text;
            _selectedCenter.Email = txtEmail.Text;

            if (InspectionCenterDAO.UpdateInspectionCenter(_selectedCenter))
            {
                // Cập nhật dữ liệu trong danh sách gốc
                var index = _allCenters.IndexOf(_selectedCenter);
                if (index >= 0)
                {
                    _allCenters[index] = new InspectionCenter
                    {
                        CenterId = _selectedCenter.CenterId,
                        Name = _selectedCenter.Name,
                        Address = _selectedCenter.Address,
                        PhoneNumber = _selectedCenter.PhoneNumber,
                        Email = _selectedCenter.Email
                    };
                }

                UpdatePagedData(); // Cập nhật lại danh sách trên UI
                MessageBox.Show("Cập nhật thành công!");
            }
            else
            {
                MessageBox.Show("Lỗi khi cập nhật!");
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCenter == null)
            {
                MessageBox.Show("Chọn một trung tâm để xóa!");
                return;
            }

            if (MessageBox.Show("Bạn có chắc muốn xóa?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (InspectionCenterDAO.DeleteInspectionCenter(_selectedCenter.CenterId))
                {
                    _allCenters.Remove(_selectedCenter);
                    UpdatePagedData();
                    MessageBox.Show("Xóa thành công!");
                }

                else
                {
                    MessageBox.Show("Lỗi khi xóa!");
                }
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            txtName.Text = string.Empty;
            txtAddress.Text = string.Empty;
            txtPhoneNumber.Text = string.Empty;
            txtEmail.Text = string.Empty;
            _selectedCenter = null; // Xóa lựa chọn hiện tại
            dgInspectionCenters.SelectedItem = null; // Bỏ chọn dòng trong DataGrid
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();

            // Nếu không nhập gì, hiển thị lại danh sách gốc
            if (string.IsNullOrWhiteSpace(keyword))
            {
                InspectionCenters.Clear();
                foreach (var center in _allInspectionCenters)
                {
                    InspectionCenters.Add(center);
                }
                return;
            }

            string searchType = (cbSearchType.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (string.IsNullOrEmpty(searchType))
            {
                MessageBox.Show("Vui lòng chọn loại tìm kiếm!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Lọc danh sách theo tiêu chí tìm kiếm
            var filteredCenters = _allInspectionCenters.Where(c =>
                (searchType == "Tên Trung Tâm" && c.Name.ToLower().Contains(keyword)) ||
                (searchType == "Địa Chỉ" && c.Address.ToLower().Contains(keyword)) ||
                (searchType == "Số Điện Thoại" && c.PhoneNumber.Contains(keyword)) ||
                (searchType == "Email" && c.Email.ToLower().Contains(keyword))
            ).ToList();

            // Cập nhật danh sách hiển thị
            InspectionCenters.Clear();
            foreach (var center in filteredCenters)
            {
                InspectionCenters.Add(center);
            }

            // Hiển thị thông báo nếu không có kết quả
            if (InspectionCenters.Count == 0)
            {
                MessageBox.Show("Không tìm thấy kết quả nào!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        


    }
}
