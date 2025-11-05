using System.Windows;
using System.Windows.Controls;

namespace RoomManagementSystem.Presentation.Views.Page.TenantManagement
{
    public partial class DeleteConfirmationView : UserControl
    {
        // --- Sự kiện cho nút Xác nhận ---

        // 1. Đăng ký một RoutedEvent mới tên là "ConfirmClick"
        public static readonly RoutedEvent ConfirmClickEvent = EventManager.RegisterRoutedEvent(
            name: "ConfirmClick",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(RoutedEventHandler),
            ownerType: typeof(DeleteConfirmationView));

        // 2. Tạo một event wrapper (giống như event bình thường) cho ConfirmClickEvent
        public event RoutedEventHandler ConfirmClick
        {
            add { AddHandler(ConfirmClickEvent, value); }
            remove { RemoveHandler(ConfirmClickEvent, value); }
        }

        // 3. Phương thức này được gọi bởi nút "Xác nhận" trong XAML
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            // Khi nút được nhấn, phát ra sự kiện "ConfirmClick"
            RaiseEvent(new RoutedEventArgs(ConfirmClickEvent));
        }


        // --- Sự kiện cho nút Đóng (dấu X) ---

        // 1. Đăng ký một RoutedEvent mới tên là "CloseClick"
        public static readonly RoutedEvent CloseClickEvent = EventManager.RegisterRoutedEvent(
            name: "CloseClick",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(RoutedEventHandler),
            ownerType: typeof(DeleteConfirmationView));

        // 2. Tạo một event wrapper cho CloseClickEvent
        public event RoutedEventHandler CloseClick
        {
            add { AddHandler(CloseClickEvent, value); }
            remove { RemoveHandler(CloseClickEvent, value); }
        }

        // 3. Phương thức này được gọi bởi nút "X" trong XAML để giải quyết lỗi
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // Khi nút được nhấn, phát ra sự kiện "CloseClick"
            RaiseEvent(new RoutedEventArgs(CloseClickEvent));
        }

        // Hàm khởi tạo của UserControl
        public DeleteConfirmationView()
        {
            InitializeComponent();
        }
    }
}