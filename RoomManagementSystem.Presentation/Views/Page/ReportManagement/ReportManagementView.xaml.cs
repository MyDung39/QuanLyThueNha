using System;
using System.Windows;
using System.Windows.Controls;

namespace RoomManagementSystem.Presentation.Views.Page.ReportManagement
{
    public partial class ReportManagementView : UserControl
    {
        private ReportMonthlyRevenueView _revenueView;
        private ReportMonthlyExpenseView _expenseView;
        private ReportMonthlyProfitView _profitView;
        private ReportRoomListView _roomListView;
        private ReportDebtListView _debtListView;

        public ReportManagementView()
        {
            InitializeComponent();
            this.Loaded += ReportManagementView_Loaded;
        }

        private void ReportManagementView_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (tabContentControl != null)
                {
                    LoadRevenueTab();
                }
            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }

        private void LoadRevenueTab()
        {
            if (tabContentControl == null) return;
            if (_revenueView == null) _revenueView = new ReportMonthlyRevenueView();
            tabContentControl.Content = _revenueView;
        }

        private void LoadExpenseTab()
        {
            if (tabContentControl == null) return;
            if (_expenseView == null) _expenseView = new ReportMonthlyExpenseView();
            tabContentControl.Content = _expenseView;
        }

        private void LoadProfitTab()
        {
            if (tabContentControl == null) return;
            if (_profitView == null) _profitView = new ReportMonthlyProfitView();
            tabContentControl.Content = _profitView;
        }

        private void LoadRoomListTab()
        {
            if (tabContentControl == null) return;
            if (_roomListView == null) _roomListView = new ReportRoomListView();
            tabContentControl.Content = _roomListView;
        }

        private void LoadDebtListTab()
        {
            if (tabContentControl == null) return;
            if (_debtListView == null) _debtListView = new ReportDebtListView();
            tabContentControl.Content = _debtListView;
        }

        private void RevenueTab_Checked(object sender, RoutedEventArgs e) => LoadRevenueTab();
        private void ExpenseTab_Checked(object sender, RoutedEventArgs e) => LoadExpenseTab();
        private void ProfitTab_Checked(object sender, RoutedEventArgs e) => LoadProfitTab();
        private void RoomListTab_Checked(object sender, RoutedEventArgs e) => LoadRoomListTab();
        private void DebtListTab_Checked(object sender, RoutedEventArgs e) => LoadDebtListTab();
    }
}
