using System.Windows.Controls;
using RoomManagementSystem.Presentation.ViewModels;

namespace RoomManagementSystem.Presentation.Views.Page.ReportManagement
{
    public partial class ReportMonthlyExpenseView : UserControl
    {
        public ReportMonthlyExpenseView()
        {
            InitializeComponent();
            DataContext = new ReportMonthlyExpenseViewModel();
        }
    }
}
