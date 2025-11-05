using System.Windows;
using System.Windows.Controls;

namespace RoomManagementSystem.Presentation.Views.Page.MaintenanceManagement
{
    public partial class DeleteMaintenanceView : UserControl
    {
        public event RoutedEventHandler ConfirmClick;
        public event RoutedEventHandler CloseClick;

        public DeleteMaintenanceView()
        {
            InitializeComponent();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            ConfirmClick?.Invoke(this, e);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseClick?.Invoke(this, e);
        }
    }
}
