using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace RoomManagementSystem.Presentation.Validation
{
    public class DecimalValidationRule : ValidationRule
    {
        public bool AllowEmpty { get; set; }
        public int MaxDecimalPlaces { get; set; } = 2;
        public bool NonNegative { get; set; } = true;
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var text = (value ?? string.Empty).ToString().Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                return AllowEmpty ? ValidationResult.ValidResult : new ValidationResult(false, "Vui lòng nhập số hợp lệ");
            }
            if (!Regex.IsMatch(text, @$"^[0-9]+([\.,][0-9]{{0,{MaxDecimalPlaces}}})?$"))
            {
                return new ValidationResult(false, "Chỉ được nhập số, tối đa 2 chữ số thập phân");
            }
            if (!decimal.TryParse(text.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out var n))
            {
                return new ValidationResult(false, "Giá trị không hợp lệ");
            }
            if (!NonNegative && n <= 0) return new ValidationResult(false, "Phải lớn hơn 0");
            if (NonNegative && n < 0) return new ValidationResult(false, "Không được âm");
            return ValidationResult.ValidResult;
        }
    }
}
