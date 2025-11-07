using System;
using System.Windows;
using System.Windows.Controls;

namespace RoomManagementSystem.Presentation.Views.Shared
{
    public partial class DeleteConfirmationModal : UserControl
    {
        public event EventHandler? ConfirmDelete;
        public event EventHandler? CancelDelete;

        public DeleteConfirmationModal()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CancelDelete?.Invoke(this, EventArgs.Empty);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CancelDelete?.Invoke(this, EventArgs.Empty);
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            ConfirmDelete?.Invoke(this, EventArgs.Empty);
        }
    }
}

