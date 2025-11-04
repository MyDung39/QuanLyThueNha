using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RoomManagementSystem.Presentation.Views.Page.HouseManagement
{
    public partial class HouseManagementView : UserControl
    {
        private class RoomData
        {
            public int Id { get; set; }
            public string RoomNumber { get; set; }
            public double Area { get; set; }
            public decimal MonthlyCost { get; set; }
            public DateTime MaintenanceDate { get; set; }
            public DateTime AvailableDate { get; set; }
            public RoomStatus Status { get; set; }
        }

        private enum RoomStatus { Available, Occupied, Scheduled }

        private readonly List<RoomData> _allRooms = new List<RoomData>();
        private readonly List<string> _allHouses = new List<string>();
        private int _roomsCurrentPage = 1;
        private int _roomsTotalPages = 1;
        private int _roomsItemsPerPage = 8;
        private string _roomsSortOption = "Mới nhất";
        private string _housesSearchKeyword = string.Empty;
        private string _roomsSearchKeyword = string.Empty;

        public HouseManagementView()
        {
            InitializeComponent();
            Loaded += HouseManagementView_Loaded;
        }

        private void ClickAwayBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Khi nhấp vào vùng nền trong suốt, tắt ToggleButton tìm kiếm
            SearchToggleButton.IsChecked = false;
        }

        private void HouseManagementView_Loaded(object sender, RoutedEventArgs e)
        {
            if (_allRooms.Count == 0)
            {
                LoadSampleRooms();
            }
            if (_allHouses.Count == 0)
            {
                LoadSampleHouses();
            }
            UpdateRoomsDisplay();
            UpdateHousesDisplay();
            
            // Đảm bảo event handlers được đăng ký sau khi Loaded
            if (SearchToggleButton != null)
            {
                SearchToggleButton.Checked -= SearchToggleButton_Checked;
                SearchToggleButton.Unchecked -= SearchToggleButton_Unchecked;
                SearchToggleButton.Checked += SearchToggleButton_Checked;
                SearchToggleButton.Unchecked += SearchToggleButton_Unchecked;
            }
        }

        private void LoadSampleRooms()
        {
            _allRooms.Clear();
            var random = new Random();
            for (int i = 1; i <= 40; i++)
            {
                var statusValues = (RoomStatus[])Enum.GetValues(typeof(RoomStatus));
                _allRooms.Add(new RoomData
                {
                    Id = i,
                    RoomNumber = $"P{i:000}",
                    Area = Math.Round(random.NextDouble() * 30 + 10, 1),
                    MonthlyCost = random.Next(1500000, 6000000),
                    MaintenanceDate = DateTime.Today.AddDays(random.Next(-60, 30)),
                    AvailableDate = DateTime.Today.AddDays(random.Next(0, 90)),
                    Status = statusValues[random.Next(statusValues.Length)]
                });
            }
        }

        private void LoadSampleHouses()
        {
            _allHouses.Clear();
            _allHouses.Add("Nhà Nguyễn Trãi");
            _allHouses.Add("Nhà Bạch Đằng");
            _allHouses.Add("Nhà Lê Lợi");
            _allHouses.Add("Nhà Phan Châu Trinh");
        }

        private void UpdateRoomsDisplay()
        {
            // Lọc theo từ khóa tìm kiếm
            IEnumerable<RoomData> filtered = _allRooms;
            if (!string.IsNullOrWhiteSpace(_roomsSearchKeyword))
            {
                string keyword = _roomsSearchKeyword.Trim().ToLower();
                filtered = _allRooms.Where(r =>
                    r.RoomNumber.ToLower().Contains(keyword) ||
                    r.Area.ToString().Contains(keyword) ||
                    r.MonthlyCost.ToString().Contains(keyword) ||
                    r.MaintenanceDate.ToString("dd/MM/yyyy").Contains(keyword) ||
                    r.AvailableDate.ToString("dd/MM/yyyy").Contains(keyword) ||
                    GetStatusText(r.Status).ToLower().Contains(keyword)
                );
            }

            // Sắp xếp
            IEnumerable<RoomData> sorted = filtered;
            switch (_roomsSortOption)
            {
                case "Số phòng":
                    sorted = filtered.OrderBy(r => r.RoomNumber);
                    break;
                case "Diện tích":
                    sorted = filtered.OrderBy(r => r.Area);
                    break;
                case "Chi phí theo tháng":
                    sorted = filtered.OrderBy(r => r.MonthlyCost);
                    break;
                case "Ngày bảo trì":
                    sorted = filtered.OrderBy(r => r.MaintenanceDate);
                    break;
                case "Ngày có sẵn":
                    sorted = filtered.OrderBy(r => r.AvailableDate);
                    break;
                case "Trạng thái":
                    sorted = filtered.OrderBy(r => r.Status);
                    break;
                default:
                    sorted = filtered.OrderByDescending(r => r.Id);
                    break;
            }

            int totalItems = sorted.Count();
            _roomsTotalPages = (int)Math.Ceiling((double)totalItems / _roomsItemsPerPage);
            if (_roomsTotalPages == 0) _roomsTotalPages = 1;
            if (_roomsCurrentPage > _roomsTotalPages) _roomsCurrentPage = Math.Max(1, _roomsTotalPages);

            var pageData = sorted
                .Skip((_roomsCurrentPage - 1) * _roomsItemsPerPage)
                .Take(_roomsItemsPerPage)
                .ToList();

            RenderRoomList(pageData);
            UpdateRoomsPaginationControls(totalItems);
        }

        private string GetStatusText(RoomStatus status)
        {
            switch (status)
            {
                case RoomStatus.Occupied: return "Đang thuê";
                case RoomStatus.Scheduled: return "Dự kiến";
                default: return "Trống";
            }
        }

        private void RenderRoomList(List<RoomData> rooms)
        {
            if (RoomListDataPanel == null)
            {
                Dispatcher.BeginInvoke(new Action(() => RenderRoomList(rooms)), System.Windows.Threading.DispatcherPriority.Loaded);
                return;
            }
            RoomListDataPanel.Children.Clear();
            foreach (var room in rooms)
            {
                RoomListDataPanel.Children.Add(CreateRoomRow(room));
            }
        }

        private Border CreateRoomRow(RoomData room)
        {
            var border = new Border
            {
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EEEEEE")),
                BorderThickness = new Thickness(0, 1, 0, 0),
                Padding = new Thickness(10)
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.5, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.5, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.5, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.5, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var checkBox = new CheckBox { VerticalAlignment = VerticalAlignment.Center };
            Grid.SetColumn(checkBox, 0);

            var roomBlock = new TextBlock { Text = room.RoomNumber, VerticalAlignment = VerticalAlignment.Center, FontWeight = FontWeights.SemiBold };
            Grid.SetColumn(roomBlock, 1);

            var areaBlock = new TextBlock { Text = $"{room.Area:0.0} m²", VerticalAlignment = VerticalAlignment.Center };
            Grid.SetColumn(areaBlock, 2);

            var costBlock = new TextBlock { Text = string.Format(System.Globalization.CultureInfo.GetCultureInfo("vi-VN"), "{0:n0} đ", room.MonthlyCost), VerticalAlignment = VerticalAlignment.Center };
            Grid.SetColumn(costBlock, 3);

            var maintenanceBlock = new TextBlock { Text = room.MaintenanceDate.ToString("dd/MM/yyyy"), VerticalAlignment = VerticalAlignment.Center };
            Grid.SetColumn(maintenanceBlock, 4);

            var availableBlock = new TextBlock { Text = room.AvailableDate.ToString("dd/MM/yyyy"), VerticalAlignment = VerticalAlignment.Center };
            Grid.SetColumn(availableBlock, 5);

            var statusBorder = new Border { CornerRadius = new CornerRadius(8), Padding = new Thickness(10, 4, 10, 4), HorizontalAlignment = HorizontalAlignment.Left };
            var statusBlock = new TextBlock { VerticalAlignment = VerticalAlignment.Center, FontSize = 12, FontWeight = FontWeights.Medium };

            string statusText; string backgroundColor; string foregroundColor;
            switch (room.Status)
            {
                case RoomStatus.Occupied: statusText = "Đang thuê"; backgroundColor = "#E9F7EF"; foregroundColor = "#00B69B"; break;
                case RoomStatus.Scheduled: statusText = "Dự kiến"; backgroundColor = "#FEF5E6"; foregroundColor = "#FF9500"; break;
                default: statusText = "Trống"; backgroundColor = "#F0F4FF"; foregroundColor = "#4A6CF7"; break;
            }
            statusBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(backgroundColor));
            statusBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(foregroundColor));
            statusBlock.Text = statusText;
            statusBorder.Child = statusBlock;
            Grid.SetColumn(statusBorder, 6);

            grid.Children.Add(checkBox);
            grid.Children.Add(roomBlock);
            grid.Children.Add(areaBlock);
            grid.Children.Add(costBlock);
            grid.Children.Add(maintenanceBlock);
            grid.Children.Add(availableBlock);
            grid.Children.Add(statusBorder);

            border.Child = grid;
            border.Tag = room;
            return border;
        }

        private void UpdateRoomsPaginationControls(int totalItems)
        {
            int startItem = totalItems == 0 ? 0 : (_roomsCurrentPage - 1) * _roomsItemsPerPage + 1;
            int endItem = Math.Min(_roomsCurrentPage * _roomsItemsPerPage, totalItems);
            if (RoomsPaginationInfoText != null)
            {
                RoomsPaginationInfoText.Text = $"Hiển thị dữ liệu từ {startItem} đến {endItem} trong tổng số {totalItems} bản ghi";
            }

            if (RoomsPrevPageButton != null) RoomsPrevPageButton.IsEnabled = _roomsCurrentPage > 1;
            if (RoomsNextPageButton != null) RoomsNextPageButton.IsEnabled = _roomsCurrentPage < _roomsTotalPages;

            if (RoomsPageButtonsPanel == null)
            {
                Dispatcher.BeginInvoke(new Action(() => UpdateRoomsPaginationControls(totalItems)), System.Windows.Threading.DispatcherPriority.Loaded);
                return;
            }
            RoomsPageButtonsPanel.Children.Clear();

            if (_roomsTotalPages <= 1) return;

            void AddPageButton(int pageNumber, bool isActive)
            {
                var button = new Button
                {
                    Content = pageNumber.ToString(),
                    Margin = new Thickness(0, 0, 8, 0),
                    Tag = pageNumber,
                    Style = (Style)FindResource("PaginationButtonStyle")
                };
                if (isActive)
                {
                    button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#707FDD"));
                    button.Foreground = Brushes.White;
                    button.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5932EA"));
                }
                button.Click += RoomsPageButton_Click;
                RoomsPageButtonsPanel.Children.Add(button);
            }

            TextBlock MakeEllipsis() => new TextBlock { Text = "...", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 8, 0) };

            if (_roomsTotalPages <= 5)
            {
                for (int i = 1; i <= _roomsTotalPages; i++) AddPageButton(i, i == _roomsCurrentPage);
            }
            else
            {
                AddPageButton(1, _roomsCurrentPage == 1);
                if (_roomsCurrentPage > 3) RoomsPageButtonsPanel.Children.Add(MakeEllipsis());
                int start = Math.Max(2, _roomsCurrentPage - 1);
                int end = Math.Min(_roomsTotalPages - 1, _roomsCurrentPage + 1);
                for (int i = start; i <= end; i++) AddPageButton(i, i == _roomsCurrentPage);
                if (_roomsCurrentPage < _roomsTotalPages - 2) RoomsPageButtonsPanel.Children.Add(MakeEllipsis());
                AddPageButton(_roomsTotalPages, _roomsCurrentPage == _roomsTotalPages);
            }
        }

        private void RoomsPrevPageButton_Click(object sender, RoutedEventArgs e)
        {
            _roomsCurrentPage = Math.Max(1, _roomsCurrentPage - 1);
            UpdateRoomsDisplay();
        }

        private void RoomsNextPageButton_Click(object sender, RoutedEventArgs e)
        {
            _roomsCurrentPage = Math.Min(_roomsTotalPages, _roomsCurrentPage + 1);
            UpdateRoomsDisplay();
        }

        private void RoomsPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && int.TryParse(button.Tag?.ToString(), out int page))
            {
                _roomsCurrentPage = page;
                UpdateRoomsDisplay();
            }
        }

        private void RoomsSortButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button sortButton)
            {
                sortButton.ContextMenu.PlacementTarget = sortButton;
                sortButton.ContextMenu.IsOpen = true;
            }
        }

        private void RoomsSortMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem item)
            {
                _roomsSortOption = item.Header?.ToString() ?? _roomsSortOption;
                if (RoomsSortButtonText != null) RoomsSortButtonText.Text = _roomsSortOption;
                _roomsCurrentPage = 1;
                UpdateRoomsDisplay();
            }
        }

        private void RoomsSelectAll_Checked(object sender, RoutedEventArgs e) => SetAllRoomCheckBoxes(true);
        private void RoomsSelectAll_Unchecked(object sender, RoutedEventArgs e) => SetAllRoomCheckBoxes(false);

        private void SetAllRoomCheckBoxes(bool isChecked)
        {
            foreach (Border border in RoomListDataPanel.Children)
            {
                var checkBox = FindVisualChild<CheckBox>(border);
                if (checkBox != null) checkBox.IsChecked = isChecked;
            }
        }

        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T t) return t;
                var inner = FindVisualChild<T>(child);
                if (inner != null) return inner;
            }
            return null;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                _housesSearchKeyword = textBox.Text ?? string.Empty;
                UpdateHousesDisplay();
            }
        }
        
        private void SearchToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            // Khi toggle được bật, focus vào TextBox và cập nhật display nếu có text
            if (SearchTextBox != null)
            {
                // Delay một chút để đảm bảo TextBox đã visible
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    SearchTextBox.Focus();
                    // Nếu có text, cập nhật display ngay
                    if (!string.IsNullOrWhiteSpace(SearchTextBox.Text))
                    {
                        _housesSearchKeyword = SearchTextBox.Text;
                        UpdateHousesDisplay();
                    }
                }), System.Windows.Threading.DispatcherPriority.Loaded);
            }
        }

        private void SearchToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            // Khi toggle được tắt, giữ lại từ khóa nhưng không xóa (để khi bật lại vẫn còn)
            // Có thể clear nếu muốn reset search
        }

        private void RoomsSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                _roomsSearchKeyword = textBox.Text ?? string.Empty;
                _roomsCurrentPage = 1; // Reset về trang đầu khi tìm kiếm
                UpdateRoomsDisplay();
            }
        }

        private void UpdateHousesDisplay()
        {
            if (HouseListBox == null) return;

            HouseListBox.Items.Clear();

            IEnumerable<string> filteredHouses = _allHouses;
            if (!string.IsNullOrWhiteSpace(_housesSearchKeyword))
            {
                string keyword = _housesSearchKeyword.Trim().ToLower();
                filteredHouses = _allHouses.Where(h => h.ToLower().Contains(keyword));
            }

            foreach (var house in filteredHouses)
            {
                var item = new ListBoxItem { Content = house };
                HouseListBox.Items.Add(item);
            }

            // Giữ nguyên item được chọn nếu có
            if (HouseListBox.Items.Count > 0 && HouseListBox.SelectedIndex == -1)
            {
                HouseListBox.SelectedIndex = 0;
            }
        }

        private void AddHouseButton_Click(object sender, RoutedEventArgs e)
        {
            if (AddHouseDialog != null)
            {
                AddHouseDialog.ClearFields();
                AddHouseDialog.Visibility = Visibility.Visible;
            }
        }

        private void EditHouseButton_Click(object sender, RoutedEventArgs e)
        {
            if (HouseListBox.SelectedItem is ListBoxItem selectedItem && selectedItem.Content is string houseName)
            {
                if (EditHouseDialog != null)
                {
                    // Lấy dữ liệu nhà hiện tại (giả sử lưu trong format: "Tên nhà|Địa chỉ|Ghi chú")
                    // Tạm thời dùng dữ liệu mẫu
                    string address = "123 đường Nguyễn Trãi, phường Thanh Xuân, Hà Nội";
                    string notes = "Dùng chung nhà bếp, có khu vực để xe";
                    
                    EditHouseDialog.LoadHouseData(houseName, address, notes);
                    EditHouseDialog.Visibility = Visibility.Visible;
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn nhà cần chỉnh sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeleteHouseButton_Click(object sender, RoutedEventArgs e)
        {
            if (HouseListBox.SelectedItem is ListBoxItem selectedItem && selectedItem.Content is string houseName)
            {
                if (DeleteHouseDialog != null)
                {
                    DeleteHouseDialog.Visibility = Visibility.Visible;
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn nhà cần xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void AddHouseDialog_HouseAdded(object? sender, string houseInfo)
        {
            if (sender is AddHouseView dialog)
            {
                // Parse thông tin: "Địa chỉ|Ghi chú"
                string[] parts = houseInfo.Split('|');
                if (parts.Length >= 1)
                {
                    string address = parts[0];
                    string notes = parts.Length > 1 ? parts[1] : string.Empty;
                    
                    // Tạo tên nhà từ địa chỉ (hoặc có thể dùng logic khác)
                    string houseName = $"Nhà {address.Split(',')[0].Trim()}";
                    
                    // Thêm vào danh sách
                    _allHouses.Add(houseName);
                    
                    // Cập nhật hiển thị
                    UpdateHousesDisplay();
                    
                    // Ẩn dialog
                    dialog.Visibility = Visibility.Collapsed;
                    dialog.ClearFields();
                }
            }
        }

        private void AddHouseDialog_Canceled(object? sender, EventArgs e)
        {
            if (sender is AddHouseView dialog)
            {
                dialog.Visibility = Visibility.Collapsed;
                dialog.ClearFields();
            }
        }

        private void EditHouseDialog_HouseUpdated(object? sender, string houseInfo)
        {
            if (sender is EditHouseView dialog)
            {
                // Parse thông tin: "Tên nhà cũ|Địa chỉ|Ghi chú"
                string[] parts = houseInfo.Split('|');
                if (parts.Length >= 2)
                {
                    string oldHouseName = parts[0];
                    string address = parts[1];
                    string notes = parts.Length > 2 ? parts[2] : string.Empty;
                    
                    // Tìm và cập nhật trong danh sách
                    int index = _allHouses.FindIndex(h => h == oldHouseName);
                    if (index >= 0)
                    {
                        // Tạo tên nhà mới từ địa chỉ (hoặc giữ nguyên tên cũ)
                        string newHouseName = $"Nhà {address.Split(',')[0].Trim()}";
                        _allHouses[index] = newHouseName;
                        
                        // Cập nhật hiển thị
                        UpdateHousesDisplay();
                        
                        // Chọn lại nhà vừa sửa
                        for (int i = 0; i < HouseListBox.Items.Count; i++)
                        {
                            if (HouseListBox.Items[i] is ListBoxItem item && item.Content.ToString() == newHouseName)
                            {
                                HouseListBox.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                    
                    // Ẩn dialog
                    dialog.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void EditHouseDialog_Canceled(object? sender, EventArgs e)
        {
            if (sender is EditHouseView dialog)
            {
                dialog.Visibility = Visibility.Collapsed;
            }
        }

        private void DeleteHouseDialog_ConfirmDelete(object? sender, EventArgs e)
        {
            if (sender is DeleteHouseView dialog)
            {
                if (HouseListBox.SelectedItem is ListBoxItem selectedItem && selectedItem.Content is string houseName)
                {
                    // Xóa khỏi danh sách
                    _allHouses.Remove(houseName);
                    
                    // Cập nhật hiển thị
                    UpdateHousesDisplay();
                    
                    // Ẩn dialog
                    dialog.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void DeleteHouseDialog_Canceled(object? sender, EventArgs e)
        {
            if (sender is DeleteHouseView dialog)
            {
                dialog.Visibility = Visibility.Collapsed;
            }
        }

        // ===================== Rooms: Buttons and Dialogs =====================
        private void AddRoomButton_Click(object sender, RoutedEventArgs e)
        {
            if (AddRoomDialog != null)
            {
                AddRoomDialog.ClearFields();
                AddRoomDialog.Visibility = Visibility.Visible;
            }
        }

        private RoomData GetSelectedRoom()
        {
            foreach (Border border in RoomListDataPanel.Children)
            {
                var checkBox = FindVisualChild<CheckBox>(border);
                if (checkBox != null && checkBox.IsChecked == true)
                {
                    return border.Tag as RoomData;
                }
            }
            return null;
        }

        private void EditRoomButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedRoom();
            if (selected == null)
            {
                MessageBox.Show("Vui lòng chọn phòng cần chỉnh sửa (tick vào ô).", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (EditRoomDialog != null)
            {
                EditRoomDialog.LoadRoomData(selected.RoomNumber,
                    selected.Area.ToString(),
                    selected.MonthlyCost.ToString(),
                    string.Empty);
                EditRoomDialog.Visibility = Visibility.Visible;
            }
        }

        private void DeleteRoomButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedRoom();
            if (selected == null)
            {
                MessageBox.Show("Vui lòng chọn phòng cần xóa (tick vào ô).", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (DeleteRoomDialog != null)
            {
                DeleteRoomDialog.Visibility = Visibility.Visible;
            }
        }

        private void AddRoomDialog_RoomAdded(object? sender, string roomInfo)
        {
            if (sender is AddRoomView dialog)
            {
                // Format: roomNumber|area|monthlyCost|notes
                var parts = roomInfo.Split('|');
                if (parts.Length >= 4)
                {
                    string roomNumber = parts[0];
                    double.TryParse(parts[1], out double area);
                    decimal.TryParse(parts[2], out decimal monthlyCost);
                    // Không có trạng thái trong form thêm (giống form sửa). Mặc định Trống
                    RoomStatus status = RoomStatus.Available;

                    _allRooms.Add(new RoomData
                    {
                        Id = _allRooms.Count > 0 ? _allRooms.Max(r => r.Id) + 1 : 1,
                        RoomNumber = roomNumber,
                        Area = area,
                        MonthlyCost = monthlyCost,
                        MaintenanceDate = DateTime.Today,
                        AvailableDate = DateTime.Today,
                        Status = status
                    });

                    UpdateRoomsDisplay();
                    dialog.Visibility = Visibility.Collapsed;
                    dialog.ClearFields();
                }
            }
        }

        private void AddRoomDialog_Canceled(object? sender, EventArgs e)
        {
            if (sender is AddRoomView dialog)
            {
                dialog.Visibility = Visibility.Collapsed;
                dialog.ClearFields();
            }
        }

        private void EditRoomDialog_RoomUpdated(object? sender, string roomInfo)
        {
            if (sender is EditRoomView dialog)
            {
                // Format: originalRoomNumber|roomNumber|area|monthlyCost|notes
                var parts = roomInfo.Split('|');
                if (parts.Length >= 4)
                {
                    string originalNumber = parts[0];
                    string newNumber = parts[1];
                    double.TryParse(parts[2], out double area);
                    decimal.TryParse(parts[3], out decimal monthlyCost);

                    var room = _allRooms.FirstOrDefault(r => r.RoomNumber == originalNumber);
                    if (room != null)
                    {
                        room.RoomNumber = newNumber;
                        room.Area = area;
                        room.MonthlyCost = monthlyCost;
                        UpdateRoomsDisplay();
                    }

                    dialog.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void EditRoomDialog_Canceled(object? sender, EventArgs e)
        {
            if (sender is EditRoomView dialog)
            {
                dialog.Visibility = Visibility.Collapsed;
            }
        }

        private void DeleteRoomDialog_ConfirmDelete(object? sender, EventArgs e)
        {
            if (sender is DeleteRoomView dialog)
            {
                var selected = GetSelectedRoom();
                if (selected != null)
                {
                    _allRooms.Remove(selected);
                    UpdateRoomsDisplay();
                }
                dialog.Visibility = Visibility.Collapsed;
            }
        }

        private void DeleteRoomDialog_Canceled(object? sender, EventArgs e)
        {
            if (sender is DeleteRoomView dialog)
            {
                dialog.Visibility = Visibility.Collapsed;
            }
        }
    }
}