using System.Windows;
using System.Windows.Controls;

namespace RoomManagementSystem.Presentation.Views.Shared
{
    public partial class SidebarView : UserControl
    {
        public SidebarView()
        {
            InitializeComponent();
        }

        // Định nghĩa các Dependency Property để có thể binding từ bên ngoài

        public bool IsUsersSelected
        {
            get { return (bool)GetValue(IsUsersSelectedProperty); }
            set { SetValue(IsUsersSelectedProperty, value); }
        }
        public static readonly DependencyProperty IsUsersSelectedProperty =
            DependencyProperty.Register("IsUsersSelected", typeof(bool), typeof(SidebarView), new PropertyMetadata(false));

        public bool IsReservationsSelected
        {
            get { return (bool)GetValue(IsReservationsSelectedProperty); }
            set { SetValue(IsReservationsSelectedProperty, value); }
        }
        public static readonly DependencyProperty IsReservationsSelectedProperty =
            DependencyProperty.Register("IsReservationsSelected", typeof(bool), typeof(SidebarView), new PropertyMetadata(false));

        public bool IsRoomsSelected
        {
            get { return (bool)GetValue(IsRoomsSelectedProperty); }
            set { SetValue(IsRoomsSelectedProperty, value); }
        }
        public static readonly DependencyProperty IsRoomsSelectedProperty =
            DependencyProperty.Register("IsRoomsSelected", typeof(bool), typeof(SidebarView), new PropertyMetadata(false));

        public bool IsBillingSelected
        {
            get { return (bool)GetValue(IsBillingSelectedProperty); }
            set { SetValue(IsBillingSelectedProperty, value); }
        }
        public static readonly DependencyProperty IsBillingSelectedProperty =
            DependencyProperty.Register("IsBillingSelected", typeof(bool), typeof(SidebarView), new PropertyMetadata(false));

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {

        }

        // Thêm các property khác cho các mục menu còn lại...
    }
}