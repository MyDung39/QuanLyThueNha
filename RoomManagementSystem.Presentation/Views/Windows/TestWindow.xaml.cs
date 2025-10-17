using System.Windows;

namespace RoomManagementSystem.Presentation.Views.Windows
{
    /// <summary>
    /// Interaction logic for TestWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        public TestWindow()
        {
            InitializeComponent();
        }

        // --- BẠN CÓ THỂ XÓA CÁC PHƯƠNG THỨC "LOADED" CŨ NẾU KHÔNG DÙNG ---
        // private void HeaderView_Loaded(object sender, RoutedEventArgs e) { }
        // private void SidebarView_Loaded(object sender, RoutedEventArgs e) { }


        // === Thêm phương thức xử lý sự kiện chuyển đổi sidebar ===
        private void HeaderView_ToggleSidebarClick(object sender, RoutedEventArgs e)
        {
            // Kiểm tra xem sidebar lớn có đang hiển thị không
            if (FullSidebar.Visibility == Visibility.Visible)
            {
                // Nếu có, thì ẩn nó đi và hiện sidebar nhỏ
                FullSidebar.Visibility = Visibility.Collapsed;
                SmallSidebar.Visibility = Visibility.Visible;
                // Thu nhỏ độ rộng của cột chứa sidebar
                SidebarColumn.Width = new GridLength(59);
            }
            else
            {
                // Nếu không, thì làm ngược lại
                FullSidebar.Visibility = Visibility.Visible;
                SmallSidebar.Visibility = Visibility.Collapsed;
                // Mở rộng lại độ rộng của cột
                SidebarColumn.Width = new GridLength(240);
            }
        }
    }
}