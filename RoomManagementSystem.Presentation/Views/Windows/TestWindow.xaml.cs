//using RoomManagementSystem.Presentation.Views.Page;
using RoomManagementSystem.Presentation.ViewModels.Windows;
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

            this.DataContext = new TestWindowViewModel();
        }


        private void HeaderView_ToggleSidebarClick(object sender, RoutedEventArgs e)
        {
            if (FullSidebar.Visibility == Visibility.Visible)
            {
                FullSidebar.Visibility = Visibility.Collapsed;
                SmallSidebar.Visibility = Visibility.Visible;
                SidebarColumn.Width = new GridLength(59);
            }
            else
            {
                FullSidebar.Visibility = Visibility.Visible;
                SmallSidebar.Visibility = Visibility.Collapsed;
                SidebarColumn.Width = new GridLength(240);
            }
        }
    }
}