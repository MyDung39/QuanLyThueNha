using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RoomManagementSystem.Presentation.Views.Page
{
    /// <summary>
    /// Interaction logic for UserManagementView.xaml
    /// </summary>
    public partial class UserManagementView : UserControl
    {
        public UserManagementView()
        {
            InitializeComponent();
        }

        // Sự kiện click cho nút chức năng (ví dụ)
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Nút chức năng 'Xem chi tiết' đã được nhấn!");
        }

        // --- LOGIC CHO NÚT "CHỌN TẤT CẢ" ---

        private void SelectAll_Checked(object sender, RoutedEventArgs e)
        {
            SetAllCheckBoxes(true);
        }

        private void SelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            SetAllCheckBoxes(false);
        }

        private void SetAllCheckBoxes(bool isChecked)
        {
            foreach (var child in TenantListDataPanel.Children)
            {
                // === SỬA LỖI Ở ĐÂY ===
                // Chúng ta cần ép kiểu (cast) 'child' thành một DependencyObject
                // trước khi truyền nó vào hàm FindVisualChild.
                if (child is DependencyObject dpo)
                {
                    var checkBox = FindVisualChild<CheckBox>(dpo);
                    if (checkBox != null)
                    {
                        checkBox.IsChecked = isChecked;
                    }
                }
                // === KẾT THÚC SỬA LỖI ===
            }
        }

        // --- LOGIC CHO NÚT SẮP XẾP MỚI ---
        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button sortButton)
            {
                sortButton.ContextMenu.PlacementTarget = sortButton;
                sortButton.ContextMenu.IsOpen = true;
            }
        }

        private void SortMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem selectedItem)
            {
                SortButtonText.Text = selectedItem.Header.ToString();
            }
        }

        // --- HÀM TRỢ GIÚP ---
        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T)
                {
                    return (T)child;
                }
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }
    }
}