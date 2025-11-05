using System.Windows.Controls;

namespace RoomManagementSystem.Presentation.Views.Page.HouseManagement
{
    public partial class HouseManagementView : UserControl
    {
        public HouseManagementView()
        {
            InitializeComponent();

            // Gán DataContext cho View này là ViewModel tương ứng
            this.DataContext = new ViewModels.HouseManagementViewModel();
        }

        /*    private void SearchToggleButton_Checked(object sender, RoutedEventArgs e)
    {
        // Khi toggle được bật, focus vào TextBox và cập nhật display nếu có text
        if (SearchTextBox != null)
        {
            // Delay một chút để đảm bảo TextBox đã visible
            Dispatcher.BeginInvoke(new Action(() =>
            {
                SearchTextBox.Focus();
                // Nếu có text, cập nhật display ngay
                if (!string.IsNullOrWhiteSpace(SearchTextBox.Text))
                {
                    _housesSearchKeyword = SearchTextBox.Text;
                    UpdateHousesDisplay();
                }
            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }
    }

    */
    }
}