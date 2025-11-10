using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RoomManagementSystem.Presentation.Converters
{
    public class SelectedRoomToFontWeightConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2)
                return FontWeights.Medium;

            var currentMaPhong = values[0]?.ToString();
            var selectedMaPhong = values[1]?.ToString();

            if (!string.IsNullOrEmpty(currentMaPhong) && !string.IsNullOrEmpty(selectedMaPhong) &&
                string.Equals(currentMaPhong, selectedMaPhong, StringComparison.OrdinalIgnoreCase))
            {
                return FontWeights.Bold;
            }

            return FontWeights.Medium;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
