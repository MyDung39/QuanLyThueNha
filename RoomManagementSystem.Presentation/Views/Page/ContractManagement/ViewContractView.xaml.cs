using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using DocumentFormat.OpenXml.Packaging;
using Word = DocumentFormat.OpenXml.Wordprocessing;
using System.Linq;

namespace RoomManagementSystem.Presentation.Views.Page.ContractManagement
{
    /// <summary>
    /// Interaction logic for ViewContractView.xaml
    /// </summary>
    public partial class ViewContractView : UserControl
    {
        public ViewContractView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Load file Word vào viewer
        /// </summary>
        public void LoadContractFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    LoadDocxFile(filePath);
                }
                else
                {
                    ShowError("Lỗi: Không tìm thấy file hợp đồng tại đường dẫn: " + filePath);
                }
            }
            catch (Exception ex)
            {
                ShowError("Đã xảy ra lỗi khi đọc file: " + ex.Message);
            }
        }

        /// <summary>
        /// Cập nhật tên hợp đồng hiển thị ở footer
        /// </summary>
        public void UpdateContractName(string contractName)
        {
            contractNameText.Text = contractName ?? "Hợp đồng";
        }

        private void LoadDocxFile(string filePath)
        {
            FlowDocument flowDoc = new FlowDocument();
            
            // Cấu hình FlowDocument như một trang giấy A4
            flowDoc.PageWidth = 794; // A4 width at 96 DPI (8.27 inches)
            flowDoc.PageHeight = 1123; // A4 height at 96 DPI (11.69 inches)
            flowDoc.PagePadding = new Thickness(72); // 1 inch margins (72 points = 1 inch at 96 DPI)
            flowDoc.ColumnWidth = flowDoc.PageWidth - flowDoc.PagePadding.Left - flowDoc.PagePadding.Right;
            flowDoc.Background = Brushes.White;
            flowDoc.FontFamily = new FontFamily("Times New Roman");
            flowDoc.FontSize = 12;

            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(filePath, false))
            {
                MainDocumentPart mainPart = wordDocument.MainDocumentPart;
                Word.Document document = mainPart.Document;

                // Đọc cài đặt trang từ Word
                var sectPr = document.Body.Elements<Word.SectionProperties>().FirstOrDefault();
                if (sectPr != null)
                {
                    var pgSz = sectPr.Elements<Word.PageSize>().FirstOrDefault();
                    var pgMar = sectPr.Elements<Word.PageMargin>().FirstOrDefault();

                    if (pgSz != null)
                    {
                        // Convert từ twips (1/1440 inch) sang pixels (96 DPI)
                        double widthInInches = (double)pgSz.Width.Value / 1440.0;
                        double heightInInches = (double)pgSz.Height.Value / 1440.0;
                        flowDoc.PageWidth = widthInInches * 96;
                        flowDoc.PageHeight = heightInInches * 96;
                    }

                    if (pgMar != null)
                    {
                        // Convert margins từ twips sang pixels
                        double leftMargin = (double)pgMar.Left.Value / 1440.0 * 96;
                        double rightMargin = (double)pgMar.Right.Value / 1440.0 * 96;
                        double topMargin = (double)pgMar.Top.Value / 1440.0 * 96;
                        double bottomMargin = (double)pgMar.Bottom.Value / 1440.0 * 96;
                        flowDoc.PagePadding = new Thickness(leftMargin, topMargin, rightMargin, bottomMargin);
                        flowDoc.ColumnWidth = flowDoc.PageWidth - leftMargin - rightMargin;
                    }
                }

                // Đọc các phần tử từ body
                foreach (var element in mainPart.Document.Body.Elements())
                {
                    if (element is Word.Paragraph wordParagraph)
                    {
                        Paragraph wpfParagraph = new Paragraph();

                        // Đọc thuộc tính paragraph
                        if (wordParagraph.ParagraphProperties != null)
                        {
                            var paraProps = wordParagraph.ParagraphProperties;
                            
                            // Alignment
                            if (paraProps.Justification != null)
                            {
                                var val = paraProps.Justification.Val;
                                if (val == Word.JustificationValues.Center)
                                    wpfParagraph.TextAlignment = System.Windows.TextAlignment.Center;
                                else if (val == Word.JustificationValues.Right)
                                    wpfParagraph.TextAlignment = System.Windows.TextAlignment.Right;
                                else if (val == Word.JustificationValues.Both)
                                    wpfParagraph.TextAlignment = System.Windows.TextAlignment.Justify;
                                else
                                    wpfParagraph.TextAlignment = System.Windows.TextAlignment.Left;
                            }

                            // Spacing
                            if (paraProps.SpacingBetweenLines != null)
                            {
                                var spacing = paraProps.SpacingBetweenLines;
                                if (spacing.Line != null && spacing.Line.Value != null)
                                {
                                    if (double.TryParse(spacing.Line.Value, out double lineSpacingValue))
                                    {
                                        double lineSpacing = lineSpacingValue / 240.0; // Convert từ twips
                                        wpfParagraph.LineHeight = lineSpacing * 12; // Base font size 12
                                    }
                                }
                            }

                            // Indentation
                            if (paraProps.Indentation != null)
                            {
                                var indent = paraProps.Indentation;
                                if (indent.Left != null && indent.Left.Value != null)
                                {
                                    if (double.TryParse(indent.Left.Value, out double leftIndentValue))
                                    {
                                        double leftIndent = leftIndentValue / 1440.0 * 96;
                                        wpfParagraph.Margin = new Thickness(leftIndent, 0, 0, 0);
                                    }
                                }
                            }
                        }

                        // Đọc các Run
                        foreach (var run in wordParagraph.Elements<Word.Run>())
                        {
                            Run wpfRun = new Run(run.InnerText);

                            if (run.RunProperties != null)
                            {
                                var runProps = run.RunProperties;

                                // Bold
                                if (runProps.Bold != null)
                                    wpfRun.FontWeight = FontWeights.Bold;

                                // Italic
                                if (runProps.Italic != null)
                                    wpfRun.FontStyle = FontStyles.Italic;

                                // Underline
                                if (runProps.Underline != null && runProps.Underline.Val != null && runProps.Underline.Val != Word.UnderlineValues.None)
                                    wpfRun.TextDecorations = TextDecorations.Underline;

                                // Strikethrough
                                if (runProps.Strike != null)
                                    wpfRun.TextDecorations = TextDecorations.Strikethrough;

                                // Font size
                                if (runProps.FontSize != null && runProps.FontSize.Val != null)
                                {
                                    if (double.TryParse(runProps.FontSize.Val.Value, out double fontSizeValue))
                                    {
                                        double fontSize = fontSizeValue / 2.0; // Convert từ half-points
                                        wpfRun.FontSize = fontSize;
                                    }
                                }

                                // Font family
                                if (runProps.RunFonts != null)
                                {
                                    string fontName = runProps.RunFonts.Ascii?.Value ?? runProps.RunFonts.EastAsia?.Value ?? "Times New Roman";
                                    wpfRun.FontFamily = new FontFamily(fontName);
                                }

                                // Color
                                if (runProps.Color != null && runProps.Color.Val != null)
                                {
                                    string colorHex = runProps.Color.Val.Value;
                                    if (colorHex.Length == 6)
                                    {
                                        byte r = Convert.ToByte(colorHex.Substring(0, 2), 16);
                                        byte g = Convert.ToByte(colorHex.Substring(2, 2), 16);
                                        byte b = Convert.ToByte(colorHex.Substring(4, 2), 16);
                                        wpfRun.Foreground = new SolidColorBrush(Color.FromRgb(r, g, b));
                                    }
                                }
                            }

                            wpfParagraph.Inlines.Add(wpfRun);
                        }

                        // Thêm paragraph vào document
                        flowDoc.Blocks.Add(wpfParagraph);
                    }
                    else if (element is Word.Table wordTable)
                    {
                        // Xử lý bảng (có thể thêm sau nếu cần)
                        Paragraph tablePlaceholder = new Paragraph(new Run("[Bảng]"));
                        tablePlaceholder.FontStyle = FontStyles.Italic;
                        tablePlaceholder.Foreground = Brushes.Gray;
                        flowDoc.Blocks.Add(tablePlaceholder);
                    }
                }
            }
            
            docViewer.Document = flowDoc;
        }

        private void ShowError(string message)
        {
            FlowDocument errorDoc = new FlowDocument();
            errorDoc.Blocks.Add(new Paragraph(new Run(message)));
            docViewer.Document = errorDoc;
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement download functionality
            MessageBox.Show("Chức năng tải xuống sẽ được triển khai sau", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement send functionality
            MessageBox.Show("Chức năng gửi sẽ được triển khai sau", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}


