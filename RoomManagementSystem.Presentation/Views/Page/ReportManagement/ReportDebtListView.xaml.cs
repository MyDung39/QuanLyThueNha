using RoomManagementSystem.Presentation.ViewModels;
using System.Windows.Controls;

namespace RoomManagementSystem.Presentation.Views.Page.ReportManagement
{
    public partial class ReportDebtListView : UserControl
    {
        public ReportDebtListView()
        {
            InitializeComponent();
            DataContext = new ReportDebtListViewModel();
        }
    }
}
