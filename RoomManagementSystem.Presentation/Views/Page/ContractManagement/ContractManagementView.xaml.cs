using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

// Sửa lại namespace cho đúng với vị trí file của bạn
namespace RoomManagementSystem.Presentation.Views.Page.ContractManagement
{
    /// <summary>
    /// Interaction logic for ContractManagementView.xaml
    /// </summary>
    public partial class ContractManagementView : UserControl
    {
        private ObservableCollection<ContractItem> _allContracts;
        private ObservableCollection<ContractItem> _filteredContracts;
        private ViewContractView _viewContractView;
        private InformationContractView _informationContractView;
        private EditContractView _editContractView;
        private NotificationContractView _notificationContractView;
        private DeleteContractView _deleteContractView;
        private AddContractView _addContractView;
        private ContractItem _currentContract;

        public ContractManagementView()
        {
            InitializeComponent();
            this.Loaded += ContractManagementView_Loaded;
            
            // Khởi tạo danh sách hợp đồng
            _allContracts = new ObservableCollection<ContractItem>
            {
                new ContractItem { ContractName = "Hợp đồng 1", TenantName = "Trần Thanh Nhã" },
                new ContractItem { ContractName = "Hợp đồng 2", TenantName = "Trần Thanh Nhã" },
                new ContractItem { ContractName = "Hợp đồng 3", TenantName = "Nguyễn Văn A" },
                new ContractItem { ContractName = "Hợp đồng 4", TenantName = "Lê Thị B" },
                new ContractItem { ContractName = "Hợp đồng 5", TenantName = "Phạm Văn C" }
            };
            
            _filteredContracts = new ObservableCollection<ContractItem>(_allContracts);
        }

        private void ContractManagementView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Load danh sách hợp đồng vào ListBox
                LoadContractsList();
                
                // Load file hợp đồng đầu tiên
                LoadFirstContract();
                
                // Load tab "Xem" mặc định - sử dụng Dispatcher để đảm bảo control đã được khởi tạo
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    LoadViewTab();
                }), System.Windows.Threading.DispatcherPriority.Loaded);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi khởi tạo: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadContractsList()
        {
            contractsListBox.Items.Clear();
            foreach (var contract in _filteredContracts)
            {
                var listBoxItem = new ListBoxItem();
                var grid = new Grid();
                
                var contractNameText = new TextBlock
                {
                    Text = contract.ContractName,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#969696")),
                    FontSize = 14
                };
                
                var tenantNameText = new TextBlock
                {
                    Text = contract.TenantName,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#969696")),
                    FontSize = 14,
                    HorizontalAlignment = HorizontalAlignment.Right
                };
                
                grid.Children.Add(contractNameText);
                grid.Children.Add(tenantNameText);
                listBoxItem.Content = grid;
                listBoxItem.Tag = contract;
                
                contractsListBox.Items.Add(listBoxItem);
            }
            
            if (contractsListBox.Items.Count > 0)
            {
                contractsListBox.SelectedIndex = 0;
            }
        }

        private void LoadFirstContract()
        {
            if (_filteredContracts.Count > 0)
            {
                var firstContract = _filteredContracts[0];
                LoadContractFile(firstContract);
            }
        }

        private void LoadContractFile(ContractItem contract)
        {
            _currentContract = contract;
            
            // Nếu đang ở tab "Xem", load file vào ViewContractView
            if (_viewContractView != null && tabContentControl != null && tabContentControl.Content == _viewContractView)
            {
                LoadContractFileToView(contract);
            }
        }

        private void LoadContractFileToView(ContractItem contract)
        {
            try
            {
                // Lấy đường dẫn thư mục của project (không phải thư mục bin/Debug)
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string projectDirectory = Path.GetFullPath(Path.Combine(baseDirectory, @"..\..\..\"));
                
                // Đường dẫn đến file BienBan.docx (có thể thay đổi dựa trên contract)
                string filePath = Path.Combine(projectDirectory, "File_dung_de_test", "BienBan.docx");

                if (_viewContractView != null)
                {
                    _viewContractView.LoadContractFile(filePath);
                    _viewContractView.UpdateContractName(contract.ContractName);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi đọc file: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadViewTab()
        {
            if (tabContentControl == null)
                return;

            if (_viewContractView == null)
            {
                _viewContractView = new ViewContractView();
            }
            
            tabContentControl.Content = _viewContractView;
            
            // Load file hợp đồng nếu có
            if (_currentContract != null)
            {
                LoadContractFileToView(_currentContract);
            }
        }

        private void ViewTab_Checked(object sender, RoutedEventArgs e)
        {
            LoadViewTab();
        }

        private void InfoTab_Checked(object sender, RoutedEventArgs e)
        {
            if (tabContentControl == null)
                return;

            // TODO: Load InfoContractView khi được tạo
            if (_informationContractView == null)
            {
                _informationContractView = new InformationContractView();
            }

            tabContentControl.Content = _informationContractView;
        }

        private void EditTab_Checked(object sender, RoutedEventArgs e)
        {
            if (tabContentControl == null)
                return;

            // TODO: Load EditContractView khi được tạo
            if (_editContractView == null)
            {
                _editContractView = new EditContractView();
            }

            tabContentControl.Content = _editContractView;
        }

        private void NotificationTab_Checked(object sender, RoutedEventArgs e)
        {
            if (tabContentControl == null)
                return;

            // TODO: Load NotificationContractView khi được tạo
            if (_notificationContractView == null)
            {
                _notificationContractView = new NotificationContractView();
            }

            tabContentControl.Content = _notificationContractView;
        }

        private void DeleteContractButton_Click(object sender, RoutedEventArgs e)
        {
            if (ModalLayer == null)
                return;

            if (_deleteContractView == null)
            {
                _deleteContractView = new DeleteContractView();
                _deleteContractView.Confirmed += (s, args) =>
                {
                    // TODO: Thực thi xóa hợp đồng hiện tại nếu có
                    HideModal();
                };
                _deleteContractView.Closed += (s, args) =>
                {
                    HideModal();
                };
            }

            ShowModal(_deleteContractView);
        }

        private void ShowModal(UserControl modalContent)
        {
            ModalLayer.Children.Clear();
            ModalLayer.Children.Add(modalContent);
            ModalLayer.Visibility = Visibility.Visible;
        }

        private void HideModal()
        {
            ModalLayer.Visibility = Visibility.Collapsed;
            ModalLayer.Children.Clear();
        }

        private void AddContractButton_Click(object sender, RoutedEventArgs e)
        {
            if (ModalLayer == null)
                return;

            if (_addContractView == null)
            {
                _addContractView = new AddContractView();
                _addContractView.Confirmed += (s, args) =>
                {
                    // TODO: Thực thi lưu hợp đồng mới
                    HideModal();
                };
                _addContractView.Closed += (s, args) =>
                {
                    HideModal();
                };
            }

            ShowModal(_addContractView);
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox && listBox.SelectedItem is ListBoxItem selectedItem)
            {
                if (selectedItem.Tag is ContractItem contract)
                {
                    // Load file hợp đồng được chọn
                    LoadContractFile(contract);
                }
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                string searchText = textBox.Text.Trim().ToLower();
                
                // Hiển thị/ẩn placeholder
                if (string.IsNullOrEmpty(searchText))
                {
                    searchPlaceholder.Visibility = Visibility.Visible;
                }
                else
                {
                    searchPlaceholder.Visibility = Visibility.Collapsed;
                }
                
                // Lọc danh sách hợp đồng
                _filteredContracts.Clear();
                
                var filtered = _allContracts.Where(c => 
                    c.TenantName.ToLower().Contains(searchText) || 
                    c.ContractName.ToLower().Contains(searchText)
                ).ToList();
                
                foreach (var contract in filtered)
                {
                    _filteredContracts.Add(contract);
                }
                
                // Cập nhật ListBox
                LoadContractsList();
            }
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && string.IsNullOrEmpty(textBox.Text))
            {
                searchPlaceholder.Visibility = Visibility.Collapsed;
            }
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && string.IsNullOrEmpty(textBox.Text))
            {
                searchPlaceholder.Visibility = Visibility.Visible;
            }
        }
    }

    // Class để lưu thông tin hợp đồng
    public class ContractItem
    {
        public string ContractName { get; set; }
        public string TenantName { get; set; }
    }
}