using System;
using System.Windows;
using System.Windows.Controls;

namespace RoomManagementSystem.Presentation.Views.Page.HouseManagement
{
    public partial class DeleteHouseView : UserControl
    {
        public event EventHandler? ConfirmDelete;
        public event EventHandler? Canceled;

        public DeleteHouseView()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Canceled?.Invoke(this, EventArgs.Empty);
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            ConfirmDelete?.Invoke(this, EventArgs.Empty);
        }
    }
}

