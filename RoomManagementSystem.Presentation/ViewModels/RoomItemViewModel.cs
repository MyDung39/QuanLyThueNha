using CommunityToolkit.Mvvm.ComponentModel;
using RoomManagementSystem.DataLayer; // Đảm bảo using đúng namespace của lớp Phong

namespace RoomManagementSystem.Presentation.ViewModels
{
    /// <summary>
    /// Lớp này "bao bọc" đối tượng Phong gốc để thêm thuộc tính IsSelected,
    /// phục vụ cho việc binding với CheckBox trên giao diện mà không làm thay đổi model gốc.
    /// </summary>
    public partial class RoomItemViewModel : ObservableObject
    {
        public Phong Phong { get; }

        [ObservableProperty]
        private bool _isSelected;

        public RoomItemViewModel(Phong phong)
        {
            Phong = phong;
        }
    }
}