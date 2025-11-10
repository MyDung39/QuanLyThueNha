using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ScottPlot; // ScottPlot v5 API

namespace RoomManagementSystem.Presentation.Views
{
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
            this.Loaded += (s, e) => InitializeCharts();
        }

        private void InitializeCharts()
        {
            var primaryBrush       = GetBrush("PrimaryHueMidBrush", "#5A6ACF");
            var primaryLightBrush  = GetBrush("PrimaryHueLightBrush", "#8593ED");
            var secondaryBrush     = GetBrush("SecondaryHueMidBrush", "#C7CEFF");
            var errorBrush         = GetBrush("MaterialDesignValidationErrorBrush", "#F2383A");
            var textBrush          = GetBrush("MaterialDesignBodyBrush", "#737B8B");
            var dividerBrush       = GetBrush("MaterialDesignDivider", "#DDE4F0");

            SetupRevenueChart(primaryBrush, textBrush, dividerBrush);
            SetupRoomStatusChart(primaryBrush, primaryLightBrush, secondaryBrush);
            SetupProfitChart(errorBrush, textBrush, dividerBrush);
        }

        /// <summary>
        /// Áp dụng style cơ bản cho biểu đồ ScottPlot 5
        /// </summary>
        private void StylePlot(Plot plot, SolidColorBrush textBrush, SolidColorBrush dividerBrush)
        {
            plot.FigureBackground.Color = ScottPlot.Color.FromHex("#00000000");
            plot.DataBackground.Color = ScottPlot.Color.FromHex("#00000000");

            if (textBrush is not null)
                plot.Axes.Color(ToScottPlotColor(textBrush));

            plot.Axes.Bottom.MajorTickStyle.Length = 0;
            plot.Axes.Left.MajorTickStyle.Length = 0;
            // Ẩn lưới theo API ScottPlot v5
            plot.HideGrid();
        }

        private void SetupRevenueChart(SolidColorBrush barColor, SolidColorBrush textBrush, SolidColorBrush dividerBrush)
        {
            StylePlot(RevenueChart.Plot, textBrush, dividerBrush);

            var values = new double[] { 52, 64, 58, 71, 69, 80 };
            var positions = new double[] { 1, 2, 3, 4, 5, 6 };
            var labels = new string[] { "Thg 1", "Thg 2", "Thg 3", "Thg 4", "Thg 5", "Thg 6" };

            var barPlot = RevenueChart.Plot.Add.Bars(positions, values);
            barPlot.Color = ToScottPlotColor(barColor);

            RevenueChart.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(positions, labels);
            RevenueChart.Plot.Axes.SetLimitsY(0, 100);
            RevenueChart.Refresh();
        }

        private void SetupRoomStatusChart(SolidColorBrush color1, SolidColorBrush color2, SolidColorBrush color3)
        {
            var plot = RoomStatusChart.Plot;
            plot.FigureBackground.Color = ScottPlot.Color.FromHex("#00000000");
            plot.DataBackground.Color = ScottPlot.Color.FromHex("#00000000");

            // Ẩn khung và trục để giống donut độc lập
            plot.Axes.Frameless();

            var pie = plot.Add.Pie(new double[] { 40, 32, 28 });
            pie.DonutFraction = 0.6;

            // Không hiển thị nhãn trên lát (đã có legend tùy ý bên dưới UI)

            pie.Slices[0].Fill.Color = ToScottPlotColor(color1);
            pie.Slices[1].Fill.Color = ToScottPlotColor(color2);
            pie.Slices[2].Fill.Color = ToScottPlotColor(color3);

            plot.HideGrid();
            RoomStatusChart.Refresh();
        }

        private void SetupProfitChart(SolidColorBrush lineColor, SolidColorBrush textBrush, SolidColorBrush dividerBrush)
        {
            StylePlot(ProfitChart.Plot, textBrush, dividerBrush);

            var values = new double[] { 18, 22, 25, 21, 27, 30 };
            var positions = new double[] { 1, 2, 3, 4, 5, 6 };
            var labels = new string[] { "Thg 1", "Thg 2", "Thg 3", "Thg 4", "Thg 5", "Thg 6" };

            var plotColor = ToScottPlotColor(lineColor);

            var line = ProfitChart.Plot.Add.ScatterLine(positions, values);
            line.Color = plotColor;
            line.LineWidth = 2;
            line.MarkerSize = 6;
            line.MarkerShape = MarkerShape.FilledCircle;

            ProfitChart.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(positions, labels);
            ProfitChart.Plot.Axes.Bottom.MajorTickStyle.Length = 0;
            ProfitChart.Plot.Axes.SetLimitsY(0, 40);
            ProfitChart.Plot.HideGrid();
            ProfitChart.Refresh();
        }

        private ScottPlot.Color ToScottPlotColor(SolidColorBrush brush, byte? alpha = null)
        {
            if (brush == null) return ScottPlot.Colors.Black; // FIX: Chỉ định rõ ScottPlot.Colors
            return new ScottPlot.Color(brush.Color.R, brush.Color.G, brush.Color.B, alpha ?? brush.Color.A);
        }

        private SolidColorBrush GetBrush(string resourceKey, string fallbackHex)
        {
            var res = TryFindResource(resourceKey) as SolidColorBrush;
            if (res != null) return res;
            var color = (System.Windows.Media.Color)ColorConverter.ConvertFromString(fallbackHex);
            return new SolidColorBrush(color);
        }
    }
}