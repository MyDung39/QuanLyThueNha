using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RoomManagementSystem.Presentation.Views.Page.TenantManagement
{
    // Lớp nội bộ để lưu trữ dữ liệu người thuê
    public class TenantData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TenantStatus Status { get; set; }
    }

    public enum TenantStatus { Occupied, PaidOut, ScheduledLeave }

    /// <summary>
    /// Interaction logic for TenantManagementView.xaml
    /// </summary>
    public partial class TenantManagementView : UserControl
    {
        private readonly List<TenantData> _allTenants = new List<TenantData>();
        private int _currentPage = 1;
        private int _totalPages = 1;
        private int _itemsPerPage = 8; // Số mục mặc định, sẽ được tính lại

        public TenantManagementView()
        {
            try
        {
            InitializeComponent();
                this.SizeChanged += OnSizeChanged;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"A critical error occurred during view initialization: \n{ex.ToString()}");
                // This is a critical failure, re-throwing might be appropriate depending on the app's error handling strategy.
                throw;
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Chỉ tải dữ liệu một lần duy nhất
            if (_allTenants.Count == 0)
            {
                LoadInitialData();
            }
            // Trì hoãn render đến sau khi visual tree sẵn sàng để đảm bảo các phần tử XAML không null
            Dispatcher.BeginInvoke(new Action(RecalculatePageSizeAndRender), System.Windows.Threading.DispatcherPriority.Loaded);
        }

        // Đảm bảo handler tồn tại cho XAML Loaded
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Gọi OnLoaded để hợp nhất luồng
            OnLoaded(sender, e);
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchTextBox.Text == "Tìm kiếm...")
            {
                SearchTextBox.Text = string.Empty;
                SearchTextBox.Foreground = (Brush)new BrushConverter().ConvertFromString("#3D3C42");
            }
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                SearchTextBox.Text = "Tìm kiếm...";
                SearchTextBox.Foreground = (Brush)new BrushConverter().ConvertFromString("#B5B7C0");
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Chỉ tính lại nếu chiều cao thay đổi
            if (e.HeightChanged)
            {
                RecalculatePageSizeAndRender();
            }
        }

        /// <summary>
        /// Tạo dữ liệu giả ban đầu
        /// </summary>
        private void LoadInitialData()
        {
            _allTenants.Clear();
            var random = new Random();
            for (int i = 1; i <= 50; i++) // Tạo 50 người thuê
            {
                var statusValues = Enum.GetValues(typeof(TenantStatus));
                _allTenants.Add(new TenantData
                {
                    Id = i,
                    Name = $"Người thuê {i}",
                    Phone = $"(84) {random.Next(900, 999)}-{random.Next(100, 999)}-{random.Next(100, 999)}",
                    Email = $"tenant{i}@example.com",
                    StartDate = DateTime.Now.AddDays(-random.Next(30, 365)),
                    EndDate = DateTime.Now.AddDays(random.Next(30, 365)),
                    Status = (TenantStatus)statusValues.GetValue(random.Next(statusValues.Length))
                });
            }
        }

        /// <summary>
        /// Tính toán lại số mục mỗi trang dựa trên chiều cao có sẵn và render lại
        /// </summary>
        private void RecalculatePageSizeAndRender()
        {
            if (MainScrollViewer.ActualHeight <= 0) return;

            // Chiều cao gần đúng của mỗi dòng
            double rowHeight = 50;
            // Chiều cao gần đúng của các phần tử không phải là danh sách (tiêu đề, thanh tìm kiếm, phân trang, padding...)
            double nonListHeight = 250;

            double availableHeight = MainScrollViewer.ActualHeight - nonListHeight;
            int newPageSize = (int)Math.Floor(availableHeight / rowHeight);

            // Đảm bảo luôn hiển thị ít nhất 1 mục
            _itemsPerPage = Math.Max(1, newPageSize);

            UpdateDisplay();
        }

        /// <summary>
        /// Hàm trung tâm để lọc, sắp xếp, phân trang và cập nhật toàn bộ giao diện
        /// </summary>
        private void UpdateDisplay()
        {
            var processedData = ProcessData();
            int totalItems = processedData.Count;
            _totalPages = (int)Math.Ceiling((double)totalItems / _itemsPerPage);
            if (_currentPage > _totalPages) _currentPage = Math.Max(1, _totalPages);

            var pageData = processedData
                .Skip((_currentPage - 1) * _itemsPerPage)
                .Take(_itemsPerPage)
                .ToList();

            // Cập nhật giao diện
            RenderTenantList(pageData);
            UpdatePaginationControls(totalItems);
        }

        /// <summary>
        /// Thực hiện lọc và sắp xếp dữ liệu từ danh sách gốc
        /// </summary>
        private List<TenantData> ProcessData()
        {
            string searchText = (SearchTextBox?.Text ?? string.Empty).ToLower();
            string sortOption = SortButtonText?.Text ?? "Mới nhất";

            IEnumerable<TenantData> filteredList = _allTenants;

            if (!string.IsNullOrWhiteSpace(searchText) && searchText != "tìm kiếm...")
            {
                filteredList = _allTenants.Where(t =>
                    t.Name.ToLower().Contains(searchText) ||
                    t.Phone.Contains(searchText) ||
                    t.Email.ToLower().Contains(searchText));
            }

            switch (sortOption)
            {
                case "Tên người thuê":
                    return filteredList.OrderBy(t => t.Name).ToList();
                case "Bắt đầu thuê":
                    return filteredList.OrderBy(t => t.StartDate).ToList();
                case "Kết thúc thuê":
                    return filteredList.OrderBy(t => t.EndDate).ToList();
                default: // Mới nhất
                    return filteredList.OrderByDescending(t => t.Id).ToList();
            }
        }

        /// <summary>
        /// Xóa và vẽ lại danh sách người thuê trên giao diện
        /// </summary>
        private void RenderTenantList(List<TenantData> tenants)
        {
            if (TenantListDataPanel == null)
            {
                // UI chưa sẵn sàng; trì hoãn lần render này
                Dispatcher.BeginInvoke(new Action(() => RenderTenantList(tenants)), System.Windows.Threading.DispatcherPriority.Loaded);
                return;
            }
            TenantListDataPanel.Children.Clear();
            foreach (var tenant in tenants)
            {
                TenantListDataPanel.Children.Add(CreateTenantRow(tenant));
            }
        }

        /// <summary>
        /// Cập nhật dòng text và các nút của thanh phân trang
        /// </summary>
        private void UpdatePaginationControls(int totalItems)
        {
            int startItem = totalItems == 0 ? 0 : (_currentPage - 1) * _itemsPerPage + 1;
            int endItem = Math.Min(_currentPage * _itemsPerPage, totalItems);
            if (PaginationInfoText != null)
            {
                PaginationInfoText.Text = $"Hiển thị dữ liệu từ {startItem} đến {endItem} trong tổng số {totalItems} bản ghi";
            }

            if (PrevPageButton != null)
            {
                PrevPageButton.IsEnabled = _currentPage > 1;
            }
            if (NextPageButton != null)
            {
                NextPageButton.IsEnabled = _currentPage < _totalPages;
            }

            if (PageButtonsPanel == null)
            {
                // UI chưa sẵn sàng; trì hoãn cập nhật phân trang
                Dispatcher.BeginInvoke(new Action(() => UpdatePaginationControls(totalItems)), System.Windows.Threading.DispatcherPriority.Loaded);
                return;
            }
            PageButtonsPanel.Children.Clear();
            var buttons = CreatePaginationButtons(_totalPages, _currentPage);
            foreach (var element in buttons)
            {
                PageButtonsPanel.Children.Add(element);
            }
        }

        /// <summary>
        /// Tạo danh sách các nút số trang và dấu "..."
        /// </summary>
        private List<UIElement> CreatePaginationButtons(int totalPages, int currentPage)
        {
            var elements = new List<UIElement>();
            if (totalPages <= 1) return elements;

            Button CreateButton(int pageNumber)
            {
                bool isActive = pageNumber == currentPage;
                var button = new Button
                {
                    Content = pageNumber.ToString(),
                    Style = (Style)FindResource("PaginationButton"),
                    Margin = new Thickness(0, 0, 8, 0),
                    Tag = pageNumber // Lưu số trang vào Tag
                };

                if (isActive)
                {
                    button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#707FDD"));
                    button.Foreground = Brushes.White;
                    button.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5932EA"));
                }

                button.Click += PageButton_Click;
                return button;
            }

            TextBlock CreateEllipsis() => new TextBlock { Text = "...", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 8, 0) };

            // Logic hiển thị các nút: 1 ... 5 6 7 ... 50
            if (totalPages <= 5)
            {
                for (int i = 1; i <= totalPages; i++)
                {
                    elements.Add(CreateButton(i));
                }
            }
            else
            {
                elements.Add(CreateButton(1));
                if (currentPage > 3)
                {
                    elements.Add(CreateEllipsis());
                }

                int start = Math.Max(2, currentPage - 1);
                int end = Math.Min(totalPages - 1, currentPage + 1);

                for (int i = start; i <= end; i++)
                {
                    elements.Add(CreateButton(i));
                }

                if (currentPage < totalPages - 2)
                {
                    elements.Add(CreateEllipsis());
                }

                elements.Add(CreateButton(totalPages));
            }
            return elements;
        }

        /// <summary>
        /// Tạo giao diện cho một dòng người thuê
        /// </summary>
        private Border CreateTenantRow(TenantData tenant)
        {
            var border = new Border
            {
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EEEEEE")),
                BorderThickness = new Thickness(0, 1, 0, 0),
                Padding = new Thickness(10)
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.5, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.5, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.5, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var checkBox = new CheckBox { VerticalAlignment = VerticalAlignment.Center };
            Grid.SetColumn(checkBox, 0);

            var nameBlock = new TextBlock { Text = tenant.Name, VerticalAlignment = VerticalAlignment.Center, FontWeight = FontWeights.SemiBold, Foreground = new SolidColorBrush(Colors.Black) };
            Grid.SetColumn(nameBlock, 1);

            var phoneBlock = new TextBlock { Text = tenant.Phone, VerticalAlignment = VerticalAlignment.Center };
            Grid.SetColumn(phoneBlock, 2);

            var emailBlock = new TextBlock { Text = tenant.Email, VerticalAlignment = VerticalAlignment.Center };
            Grid.SetColumn(emailBlock, 3);

            var startDateBlock = new TextBlock { Text = tenant.StartDate.ToString("dd/MM/yyyy"), VerticalAlignment = VerticalAlignment.Center };
            Grid.SetColumn(startDateBlock, 4);

            var endDateBlock = new TextBlock { Text = tenant.EndDate.ToString("dd/MM/yyyy"), VerticalAlignment = VerticalAlignment.Center };
            Grid.SetColumn(endDateBlock, 5);

            // Status with colored background
            var statusBorder = new Border { CornerRadius = new CornerRadius(8), Padding = new Thickness(10, 4, 10, 4), HorizontalAlignment = HorizontalAlignment.Left };
            var statusBlock = new TextBlock { Text = tenant.Status.ToString(), VerticalAlignment = VerticalAlignment.Center, FontSize = 12, FontWeight = FontWeights.Medium };

            string statusText;
            string backgroundColor;
            string foregroundColor;

            switch (tenant.Status)
            {
                case TenantStatus.Occupied:
                    statusText = "Đang thuê";
                    backgroundColor = "#E9F7EF";
                    foregroundColor = "#00B69B";
                    break;
                case TenantStatus.PaidOut:
                    statusText = "Đã trả phòng";
                    backgroundColor = "#FFF2F2";
                    foregroundColor = "#DF0404";
                    break;
                case TenantStatus.ScheduledLeave:
                default:
                    statusText = "Hẹn trả";
                    backgroundColor = "#FEF5E6";
                    foregroundColor = "#FF9500";
                    break;
            }

            statusBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(backgroundColor));
            statusBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(foregroundColor));
            statusBlock.Text = statusText;

            statusBorder.Child = statusBlock;
            Grid.SetColumn(statusBorder, 6);

            grid.Children.Add(checkBox);
            grid.Children.Add(nameBlock);
            grid.Children.Add(phoneBlock);
            grid.Children.Add(emailBlock);
            grid.Children.Add(startDateBlock);
            grid.Children.Add(endDateBlock);
            grid.Children.Add(statusBorder);

            border.Child = grid;
            return border;
        }

        #region Event Handlers
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e) => UpdateDisplay();
        private void SortMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem item)
            {
                SortButtonText.Text = item.Header.ToString();
                _currentPage = 1; // Reset về trang đầu khi sắp xếp
                UpdateDisplay();
            }
        }
        private void PrevPageButton_Click(object sender, RoutedEventArgs e)
        {
            _currentPage--;
            UpdateDisplay();
        }
        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            _currentPage++;
            UpdateDisplay();
        }
        private void PageButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int pageNumber)
            {
                _currentPage = pageNumber;
                UpdateDisplay();
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Nút 'Xem chi tiết' đã được nhấn!");
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            ModalOverlay.Visibility = Visibility.Visible;
            EditTenantModal.Visibility = Visibility.Visible;
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            ModalOverlay.Visibility = Visibility.Visible;
            AddTenantModal.Visibility = Visibility.Visible;
        }
        private void CloseEditModal_Click(object sender, RoutedEventArgs e)
        {
            ModalOverlay.Visibility = Visibility.Collapsed;
            EditTenantModal.Visibility = Visibility.Collapsed;
        }
        private void CloseAddModal_Click(object sender, RoutedEventArgs e)
        {
            ModalOverlay.Visibility = Visibility.Collapsed;
            AddTenantModal.Visibility = Visibility.Collapsed;
        }
        private void SelectAll_Checked(object sender, RoutedEventArgs e) => SetAllCheckBoxesInPage(true);
        private void SelectAll_Unchecked(object sender, RoutedEventArgs e) => SetAllCheckBoxesInPage(false);
        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button sortButton)
            {
                sortButton.ContextMenu.PlacementTarget = sortButton;
                sortButton.ContextMenu.IsOpen = true;
            }
        }
        #endregion

        #region Helper Methods
        private void SetAllCheckBoxesInPage(bool isChecked)
        {
            foreach (Border border in TenantListDataPanel.Children)
            {
                var checkBox = FindVisualChild<CheckBox>(border);
                    if (checkBox != null)
                    {
                        checkBox.IsChecked = isChecked;
                    }
                }
            }
        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T t) return t;
                var childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null) return childOfChild;
            }
            return null;
        }
        #endregion
    }
}