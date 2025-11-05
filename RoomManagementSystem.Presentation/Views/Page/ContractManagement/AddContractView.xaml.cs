using System;
using System.Windows;
using System.Windows.Controls;

namespace RoomManagementSystem.Presentation.Views.Page.ContractManagement
{
    public partial class AddContractView : UserControl
    {
        public event EventHandler Confirmed;
        public event EventHandler Closed;

        public AddContractView()
        {
            InitializeComponent();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            Confirmed?.Invoke(this, EventArgs.Empty);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }
    }
}
