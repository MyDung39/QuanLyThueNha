using System.Windows;
using System.Windows.Controls;

namespace RoomManagementSystem.Presentation.Views.Page.MaintenanceManagement
{
    public partial class EditMaintenanceView : UserControl
    {
        public event RoutedEventHandler CloseClick;
        public event RoutedEventHandler SaveClick;

        public EditMaintenanceView()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseClick?.Invoke(this, e);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveClick?.Invoke(this, e);
        }
    }
}
