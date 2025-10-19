using System.Windows;
using System.Windows.Controls;

namespace RoomManagementSystem.Presentation.Views.Page.TenantManagement
{
    public partial class EditTenantView : UserControl
    {
        // 1. Định nghĩa một Routed Event mới
        public static readonly RoutedEvent CloseClickEvent = EventManager.RegisterRoutedEvent(
            name: "CloseClick",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(RoutedEventHandler),
            ownerType: typeof(EditTenantView));

        // 2. Tạo một trình bao bọc sự kiện .NET
        public event RoutedEventHandler CloseClick
        {
            add { AddHandler(CloseClickEvent, value); }
            remove { RemoveHandler(CloseClickEvent, value); }
        }

        public EditTenantView()
        {
            InitializeComponent();
        }

        // 3. Khi nút Đóng hoặc Hủy được nhấn, phát ra sự kiện
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(CloseClickEvent));
        }
    }
}