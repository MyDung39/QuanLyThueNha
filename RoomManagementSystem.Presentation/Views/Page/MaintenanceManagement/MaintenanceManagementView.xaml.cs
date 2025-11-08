using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace RoomManagementSystem.Presentation.Views.Page.MaintenanceManagement
{
    public partial class MaintenanceManagementView : UserControl
    {
        private ObservableCollection<MaintenanceItem> _allItems = new();
        private ObservableCollection<MaintenanceItem> _pageItems = new();
        private int _pageSize = 8;
        private int _currentPage = 1;
        private int _totalPages = 1;

        public MaintenanceManagementView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Defer to ensure all named elements are created
            Dispatcher.BeginInvoke(new Action(() =>
            {
                LoadSampleData();
                ApplyPagination();
                RenderList();
            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }

        private void LoadSampleData()
        {
            _allItems.Clear();
            _allItems.Add(new MaintenanceItem { Room = "APPLE-001", Tenant = "Trần Thanh Nhã", Description = "Hỏng vòi hoa sen", RequestDate = DateTime.Parse("2024-03-20"), CompleteDate = DateTime.Parse("2024-03-20"), Status = "Đã xử lý", Cost = 200000 });
            _allItems.Add(new MaintenanceItem { Room = "BANANA-001", Tenant = "Lê Công Bảo", Description = "Mất nước", RequestDate = DateTime.Parse("2024-03-21"), CompleteDate = DateTime.Parse("2024-03-21"), Status = "Chưa xử lý", Cost = 0 });
            _allItems.Add(new MaintenanceItem { Room = "ORANGE-101", Tenant = "Mỹ Dung", Description = "Điện chập chờn", RequestDate = DateTime.Parse("2024-04-02"), CompleteDate = null, Status = "Đang xử lý", Cost = 0 });
        }

        private void ApplyPagination()
        {
            _totalPages = Math.Max(1, (int)Math.Ceiling(_allItems.Count / (double)_pageSize));
            _currentPage = Math.Min(Math.Max(1, _currentPage), _totalPages);
            _pageItems = new ObservableCollection<MaintenanceItem>(_allItems.Skip((_currentPage - 1) * _pageSize).Take(_pageSize));

            if (PaginationInfoText != null)
                PaginationInfoText.Text = $"Trang {_currentPage} / {_totalPages}";
            RenderPageButtons();
        }

        private void RenderPageButtons()
        {
            if (PageButtonsPanel == null) return;
            PageButtonsPanel.Children.Clear();
            for (int i = 1; i <= _totalPages; i++)
            {
                var btn = new Button { Content = i.ToString(), Margin = new Thickness(0, 0, 8, 0) };
                // Apply same style as TenantManagement
                if (TryFindResource("PaginationButton") is Style pgStyle)
                {
                    btn.Style = pgStyle;
                }
                btn.Click += (s, e) => { _currentPage = int.Parse((string)((Button)s).Content); ApplyPagination(); RenderList(); };
                PageButtonsPanel.Children.Add(btn);
            }
        }

        private void RenderList()
        {
            if (MaintenanceListDataPanel == null) return;
            MaintenanceListDataPanel.Children.Clear();

            foreach (var item in _pageItems)
            {
                var row = new Grid { Margin = new Thickness(10, 0, 10, 0), Height = 48 };
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50) });
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.5, GridUnitType.Star) });
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.5, GridUnitType.Star) });
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.5, GridUnitType.Star) });
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.5, GridUnitType.Star) });
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                var cb = new CheckBox { VerticalAlignment = VerticalAlignment.Center };
                Grid.SetColumn(cb, 0);
                row.Children.Add(cb);

                row.Children.Add(MkText(item.Room, 1));
                row.Children.Add(MkText(item.Tenant, 2));
                row.Children.Add(MkText(item.Description, 3));
                row.Children.Add(MkText(item.RequestDate?.ToString("yyyy-MM-dd"), 4));
                row.Children.Add(MkText(item.CompleteDate?.ToString("yyyy-MM-dd"), 5));
                row.Children.Add(MkStatus(item.Status, 6));
                row.Children.Add(MkText(item.Cost.ToString(CultureInfo.InvariantCulture), 7));

                var sep = new Line
                {
                    X1 = 0, X2 = 9999, Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0xEE, 0xEE, 0xEE)), StrokeThickness = 1,
                    VerticalAlignment = VerticalAlignment.Bottom
                };
                var wrap = new Grid();
                wrap.Children.Add(row);
                wrap.Children.Add(sep);

                MaintenanceListDataPanel.Children.Add(wrap);
            }
        }

        private FrameworkElement MkText(string text, int col)
        {
            var tb = new TextBlock { Text = text, VerticalAlignment = VerticalAlignment.Center, Foreground = new SolidColorBrush(Color.FromRgb(0x29, 0x2D, 0x32)), FontSize = 14, FontWeight = FontWeights.Medium };
            Grid.SetColumn(tb, col);
            return tb;
        }

        private FrameworkElement MkStatus(string status, int col)
        {
            var border = new Border
            {
                CornerRadius = new CornerRadius(11),
                Height = 22,
                Padding = new Thickness(8, 0, 8, 0),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                MinWidth = 71
            };
            var tb = new TextBlock { VerticalAlignment = VerticalAlignment.Center, FontSize = 11, FontWeight = FontWeights.Medium };

            if (string.Equals(status, "Đã xử lý", StringComparison.OrdinalIgnoreCase))
            {
                border.BorderBrush = new SolidColorBrush(Color.FromRgb(0x14, 0x9D, 0x52));
                border.BorderThickness = new Thickness(1);
                tb.Text = "Đã xử lý";
                tb.Foreground = border.BorderBrush;
            }
            else if (string.Equals(status, "Chưa xử lý", StringComparison.OrdinalIgnoreCase))
            {
                border.BorderBrush = new SolidColorBrush(Color.FromRgb(0xEC, 0x52, 0x52));
                border.BorderThickness = new Thickness(1);
                tb.Text = "Chưa xử lý";
                tb.Foreground = border.BorderBrush;
            }
            else
            {
                border.BorderBrush = new SolidColorBrush(Color.FromRgb(0xFF, 0xA5, 0x00));
                border.BorderThickness = new Thickness(1);
                tb.Text = status;
                tb.Foreground = border.BorderBrush;
            }

            border.Child = tb;
            Grid.SetColumn(border, col);
            return border;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterAndRefresh();
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchTextBox.Text == "Tìm kiếm...") SearchTextBox.Text = string.Empty;
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text)) SearchTextBox.Text = "Tìm kiếm...";
        }

        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            SortButton.ContextMenu.IsOpen = true;
        }

        private void SortMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem mi)
            {
                SortButtonText.Text = mi.Header?.ToString();
                ApplySort();
                ApplyPagination();
                RenderList();
            }
        }

        private void ApplySort()
        {
            var key = SortButtonText != null ? SortButtonText.Text : "Mới nhất";
            IEnumerable<MaintenanceItem> sorted = _allItems;
            if (key == "Phòng") sorted = _allItems.OrderBy(x => x.Room);
            else if (key == "Ngày yêu cầu") sorted = _allItems.OrderByDescending(x => x.RequestDate);
            else if (key == "Ngày hoàn thành") sorted = _allItems.OrderByDescending(x => x.CompleteDate);
            else sorted = _allItems.OrderByDescending(x => x.RequestDate);
            _allItems = new ObservableCollection<MaintenanceItem>(sorted);
        }

        private void FilterAndRefresh()
        {
            var q = SearchTextBox.Text?.Trim();
            IEnumerable<MaintenanceItem> data = _allItems;
            if (!string.IsNullOrWhiteSpace(q) && q != "Tìm kiếm...")
            {
                q = q.ToLowerInvariant();
                data = data.Where(x => (x.Room ?? "").ToLowerInvariant().Contains(q)
                                     || (x.Tenant ?? "").ToLowerInvariant().Contains(q)
                                     || (x.Description ?? "").ToLowerInvariant().Contains(q));
            }
            _allItems = new ObservableCollection<MaintenanceItem>(data);
            _currentPage = 1;
            ApplyPagination();
            RenderList();
        }

        private void PrevPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                ApplyPagination();
                RenderList();
            }
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage < _totalPages)
            {
                _currentPage++;
                ApplyPagination();
                RenderList();
            }
        }

        private void SelectAll_Checked(object sender, RoutedEventArgs e)
        {
            ToggleAllCheckboxes(true);
        }

        private void SelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            ToggleAllCheckboxes(false);
        }

        private void ToggleAllCheckboxes(bool isChecked)
        {
            foreach (var child in MaintenanceListDataPanel.Children)
            {
                if (child is Grid wrap && wrap.Children.Count > 0 && wrap.Children[0] is Grid row)
                {
                    foreach (var element in row.Children)
                    {
                        if (element is CheckBox cb)
                        {
                            cb.IsChecked = isChecked;
                            break;
                        }
                    }
                }
            }
        }

        private void ViewButton_Click(object sender, RoutedEventArgs e) { /* optional */ }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            ModalOverlay.Visibility = Visibility.Visible;
            EditMaintenanceModal.Visibility = Visibility.Visible;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            ModalOverlay.Visibility = Visibility.Visible;
            DeleteMaintenanceModal.Visibility = Visibility.Visible;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            ModalOverlay.Visibility = Visibility.Visible;
            AddMaintenanceModal.Visibility = Visibility.Visible;
        }

        private void CloseEditModal_Click(object sender, RoutedEventArgs e)
        {
            EditMaintenanceModal.Visibility = Visibility.Collapsed;
            ModalOverlay.Visibility = Visibility.Collapsed;
        }

        private void SaveEditModal_Click(object sender, RoutedEventArgs e)
        {
            EditMaintenanceModal.Visibility = Visibility.Collapsed;
            ModalOverlay.Visibility = Visibility.Collapsed;
        }

        private void CloseAddModal_Click(object sender, RoutedEventArgs e)
        {
            AddMaintenanceModal.Visibility = Visibility.Collapsed;
            ModalOverlay.Visibility = Visibility.Collapsed;
        }

        private void AddConfirm_Click(object sender, RoutedEventArgs e)
        {
            AddMaintenanceModal.Visibility = Visibility.Collapsed;
            ModalOverlay.Visibility = Visibility.Collapsed;
        }

        private void DeleteConfirmation_ConfirmClick(object sender, RoutedEventArgs e)
        {
            DeleteMaintenanceModal.Visibility = Visibility.Collapsed;
            ModalOverlay.Visibility = Visibility.Collapsed;
        }

        private void DeleteClose_Click(object sender, RoutedEventArgs e)
        {
            DeleteMaintenanceModal.Visibility = Visibility.Collapsed;
            ModalOverlay.Visibility = Visibility.Collapsed;
        }

        // Alias to match XAML: ConfirmClick="DeleteConfirm_Click"
        private void DeleteConfirm_Click(object sender, RoutedEventArgs e)
        {
            DeleteMaintenanceModal.Visibility = Visibility.Collapsed;
            ModalOverlay.Visibility = Visibility.Collapsed;
        }
    }

    public class MaintenanceItem
    {
        public string Room { get; set; }
        public string Tenant { get; set; }
        public string Description { get; set; }
        public DateTime? RequestDate { get; set; }
        public DateTime? CompleteDate { get; set; }
        public string Status { get; set; }
        public decimal Cost { get; set; }
    }
}
