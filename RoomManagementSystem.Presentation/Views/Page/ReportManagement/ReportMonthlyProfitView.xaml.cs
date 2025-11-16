using RoomManagementSystem.Presentation.ViewModels;
using System.Windows.Controls;

namespace RoomManagementSystem.Presentation.Views.Page.ReportManagement
{
    public partial class ReportMonthlyProfitView : UserControl
    {
        public ReportMonthlyProfitView()
        {
            InitializeComponent();
            DataContext = new ReportMonthlyProfitViewModel();
        }
    }
}
