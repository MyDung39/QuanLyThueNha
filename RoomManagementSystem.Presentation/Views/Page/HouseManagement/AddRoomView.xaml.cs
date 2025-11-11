using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace RoomManagementSystem.Presentation.Views.Page.HouseManagement
{
    public partial class AddRoomView : UserControl
    {
        public AddRoomView()
        {
            InitializeComponent();
        }

        // Regex to validate complete decimal numbers (for paste validation)
        private static readonly Regex CompleteNumericRegex = new Regex(@"^[0-9]+([\.\,][0-9]{0,2})?$", RegexOptions.Compiled);

        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null)
            {
                e.Handled = true;
                return;
            }

            // Only allow digits (0-9), dot (.), and comma (,)
            bool isDigit = char.IsDigit(e.Text, 0);
            bool isDot = e.Text == ".";
            bool isComma = e.Text == ",";

            if (!isDigit && !isDot && !isComma)
            {
                e.Handled = true; // Block any other character
                return;
            }

            // If it's a dot or comma, check if one already exists
            if ((isDot || isComma) && (tb.Text.Contains(".") || tb.Text.Contains(",")))
            {
                e.Handled = true; // Block second decimal separator
                return;
            }

            // Simulate the text after this character is inserted
            int selStart = tb.SelectionStart;
            int selLength = tb.SelectionLength;
            string currentText = tb.Text ?? string.Empty;
            string prospectiveText = currentText.Remove(selStart, selLength).Insert(selStart, e.Text);

            // Check if there are more than 2 decimal places
            int decimalIndex = prospectiveText.IndexOfAny(new[] { '.', ',' });
            if (decimalIndex >= 0)
            {
                int decimalPlaces = prospectiveText.Length - decimalIndex - 1;
                if (decimalPlaces > 2)
                {
                    e.Handled = true; // Block if more than 2 decimal places
                    return;
                }
            }
        }

        private void NumericTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.UnicodeText))
            {
                var pastedText = (string)e.DataObject.GetData(DataFormats.UnicodeText);
                
                // Validate that pasted text is a valid decimal number
                if (string.IsNullOrWhiteSpace(pastedText) || !CompleteNumericRegex.IsMatch(pastedText))
                {
                    e.CancelCommand(); // Block invalid paste
                }
            }
            else
            {
                e.CancelCommand(); // Block non-text paste
            }
        }

        // Extra safety: sanitize any invalid text that slips through (e.g. IME or programmatic changes)
        private void NumericTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is not TextBox tb) return;

            string text = tb.Text ?? string.Empty;
            int sepCount = 0;
            char sepChar = '\0';
            var cleaned = new System.Text.StringBuilder();
            foreach (char c in text)
            {
                if (char.IsDigit(c)) { cleaned.Append(c); continue; }
                if ((c == '.' || c == ',') && sepCount == 0)
                {
                    cleaned.Append(c);
                    sepCount = 1;
                    sepChar = c;
                }
            }

            if (sepCount == 1)
            {
                var parts = cleaned.ToString().Split(new[] { '.', ',' });
                if (parts.Length == 2 && parts[1].Length > 2)
                {
                    cleaned.Length = 0;
                    cleaned.Append(parts[0]);
                    cleaned.Append(sepChar);
                    cleaned.Append(parts[1].Substring(0, 2));
                }
            }

            string sanitized = cleaned.ToString();
            if (!sanitized.Equals(text))
            {
                int caret = tb.CaretIndex;
                tb.Text = sanitized;
                tb.CaretIndex = Math.Min(caret, tb.Text.Length);
            }
        }

        private void NumericTextBox_LostFocus(object sender, RoutedEventArgs e)
{
    if (sender is not TextBox tb) return;
    var text = (tb.Text ?? string.Empty).Trim();
    if (!System.Text.RegularExpressions.Regex.IsMatch(text, @"^[0-9]+([\.,][0-9]{0,2})?$"))
    {
        MessageBox.Show("Bạn nhập sai định dạng, chỉ được nhập số (tối đa 2 số thập phân)!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
    }
    if (tb.Name == "AreaTextBox")
    {
        if (decimal.TryParse(text.Replace(',', '.'), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out var n))
        {
            if (n <= 0)
            {
                MessageBox.Show("Diện tích phải lớn hơn 0!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }
    }
    if (tb.Name == "MonthlyCostTextBox")
    {
        if (!string.IsNullOrWhiteSpace(text) && decimal.TryParse(text.Replace(',', '.'), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out var n))
        {
            if (n < 0)
            {
                MessageBox.Show("Chi phí không được âm!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }
    }
}

    }
}