using System;
using System.Windows;
using System.Windows.Controls;

namespace RoomManagementSystem.Presentation.Views.Page.HouseManagement
{
    public partial class EditRoomView : UserControl
    {
        public event EventHandler<string>? RoomUpdated;
        public event EventHandler? Canceled;

        private string _originalRoomNumber = string.Empty;

        public EditRoomView()
        {
            InitializeComponent();
        }

        public void LoadRoomData(string roomNumber, string area, string monthlyCost, string notes)
        {
            _originalRoomNumber = roomNumber;
            RoomNumberTextBox.Text = roomNumber;
            AreaTextBox.Text = area;
            MonthlyCostTextBox.Text = monthlyCost;
            NotesTextBox.Text = notes;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Canceled?.Invoke(this, EventArgs.Empty);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
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

            string payload = $"{_originalRoomNumber}|{roomNumber}|{area}|{monthlyCost}|{notes}";
            RoomUpdated?.Invoke(this, payload);
        }
    }
}


