using System.Windows;
using System.Windows.Controls;

namespace RoomManagementSystem.Presentation.Views.Page.TenantManagement
{
    public partial class EditTenantView : UserControl
    {
        // --- Sự kiện để đóng Modal ---
        public static readonly RoutedEvent CloseClickEvent = EventManager.RegisterRoutedEvent(
            "CloseClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(EditTenantView));

        public event RoutedEventHandler CloseClick
        {
            add { AddHandler(CloseClickEvent, value); }
            remove { RemoveHandler(CloseClickEvent, value); }
        }

        // --- Sự kiện để Lưu ---
        public static readonly RoutedEvent SaveClickEvent = EventManager.RegisterRoutedEvent(
            "SaveClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(EditTenantView));

        public event RoutedEventHandler SaveClick
        {
            add { AddHandler(SaveClickEvent, value); }
            remove { RemoveHandler(SaveClickEvent, value); }
        }

        public EditTenantView()
        {
            InitializeComponent();
        }

        // Được gọi khi nhấn nút (X) hoặc Hủy
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // Phát tín hiệu "đóng"
            RaiseEvent(new RoutedEventArgs(CloseClickEvent));
        }

        // Được gọi khi nhấn nút "Lưu"
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // ... (Thêm logic lấy dữ liệu từ các TextBox và lưu lại ở đây) ...

            MessageBox.Show("Đã lưu thông tin người thuê!"); // Thông báo ví dụ

            // Sau khi xử lý xong, phát tín hiệu "đóng"
            RaiseEvent(new RoutedEventArgs(CloseClickEvent));
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}