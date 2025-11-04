using System;
using System.Windows;
using System.Windows.Controls;

namespace RoomManagementSystem.Presentation.Views.Page.HouseManagement
{
    public partial class EditHouseView : UserControl
    {
        public event EventHandler<string>? HouseUpdated;
        public event EventHandler? Canceled;

        private string _originalHouseName = string.Empty;

        public EditHouseView()
        {
            InitializeComponent();
        }

        public void LoadHouseData(string houseName, string address, string notes)
        {
            _originalHouseName = houseName;
            AddressTextBox.Text = address;
            NotesTextBox.Text = notes;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Canceled?.Invoke(this, EventArgs.Empty);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string address = AddressTextBox.Text?.Trim() ?? string.Empty;
            string notes = NotesTextBox.Text?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(address))
            {
                MessageBox.Show("Vui lòng nhập địa chỉ nhà.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string houseInfo = $"{_originalHouseName}|{address}|{notes}";
            HouseUpdated?.Invoke(this, houseInfo);
        }

        private void MapButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement map selection functionality
            MessageBox.Show("Chức năng chọn vị trí trên bản đồ sẽ được triển khai sau.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}

