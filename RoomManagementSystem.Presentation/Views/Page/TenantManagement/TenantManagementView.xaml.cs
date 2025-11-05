using System.Windows.Controls;


namespace RoomManagementSystem.Presentation.Views.Page.TenantManagement
{
    public partial class TenantManagementView : UserControl
    {
        public TenantManagementView()
        {
            InitializeComponent();

            // XÓA HOẶC VÔ HIỆU HÓA HOÀN TOÀN DÒNG NÀY.
            // ĐÂY CHÍNH LÀ NGUYÊN NHÂN GÂY LỖI.
            // this.DataContext = new ViewModels.TenantManagementViewModel(); 
        }


        


        // Các sự kiện này nên để trống
        private void SelectAll_Checked(object sender, System.Windows.RoutedEventArgs e) { }
        private void SelectAll_Unchecked(object sender, System.Windows.RoutedEventArgs e) { }
    }
}