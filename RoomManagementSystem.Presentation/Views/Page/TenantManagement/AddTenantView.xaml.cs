using System.Windows;
using System.Windows.Controls;

namespace RoomManagementSystem.Presentation.Views.Page.TenantManagement
{
    public partial class AddTenantView : UserControl
    {
        public static readonly RoutedEvent CloseClickEvent = EventManager.RegisterRoutedEvent(
            name: "CloseClick",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(RoutedEventHandler),
            ownerType: typeof(AddTenantView));

        public event RoutedEventHandler CloseClick
        {
            add { AddHandler(CloseClickEvent, value); }
            remove { RemoveHandler(CloseClickEvent, value); }
        }

        public AddTenantView()
        {
            InitializeComponent();
        }

        // Sự kiện cho nút Đóng
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(CloseClickEvent));
        }

        // Sự kiện cho nút Thêm
        public static readonly RoutedEvent AddClickEvent = EventManager.RegisterRoutedEvent(
            name: "AddClick",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(RoutedEventHandler),
            ownerType: typeof(AddTenantView));

        public event RoutedEventHandler AddClick
        {
            add { AddHandler(AddClickEvent, value); }
            remove { RemoveHandler(AddClickEvent, value); }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(AddClickEvent));
        }
    }
}

