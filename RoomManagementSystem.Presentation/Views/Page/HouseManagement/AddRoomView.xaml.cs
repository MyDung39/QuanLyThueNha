using System;
using System.Windows;
using System.Windows.Controls;

namespace RoomManagementSystem.Presentation.Views.Page.HouseManagement
{
    public partial class AddRoomView : UserControl
    {
        public event EventHandler<string>? RoomAdded;
        public event EventHandler? Canceled;

        public AddRoomView()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Canceled?.Invoke(this, EventArgs.Empty);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string roomNumber = RoomNumberTextBox.Text?.Trim() ?? string.Empty;
            string area = AreaTextBox.Text?.Trim() ?? string.Empty;
            string monthlyCost = MonthlyCostTextBox.Text?.Trim() ?? string.Empty;
            string notes = NotesTextBox.Text?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(roomNumber))
            {
                MessageBox.Show("Vui lòng nhập số phòng.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string payload = $"{roomNumber}|{area}|{monthlyCost}|{notes}";
            RoomAdded?.Invoke(this, payload);
        }

        public void ClearFields()
        {
            AreaTextBox.Text = string.Empty;
            RoomNumberTextBox.Text = string.Empty;
            MonthlyCostTextBox.Text = string.Empty;
            NotesTextBox.Text = string.Empty;
        }

    }
}


