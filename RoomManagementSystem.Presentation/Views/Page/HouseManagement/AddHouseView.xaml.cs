using System;
using System.Windows;
using System.Windows.Controls;

namespace RoomManagementSystem.Presentation.Views.Page.HouseManagement
{
    public partial class AddHouseView : UserControl
    {
        public event EventHandler<string>? HouseAdded;
        public event EventHandler? Canceled;

        public AddHouseView()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Canceled?.Invoke(this, EventArgs.Empty);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string address = AddressTextBox.Text?.Trim() ?? string.Empty;
            string notes = NotesTextBox.Text?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(address))
            {
                MessageBox.Show("Vui lòng nhập địa chỉ nhà.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string houseInfo = $"{address}|{notes}";
            HouseAdded?.Invoke(this, houseInfo);
        }

        private void MapButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement map selection functionality
            MessageBox.Show("Chức năng chọn vị trí trên bản đồ sẽ được triển khai sau.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ClearFields()
        {
            AddressTextBox.Text = string.Empty;
            NotesTextBox.Text = string.Empty;
        }

        private void AddressTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}

