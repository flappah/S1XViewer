using LiveCharts;
using System;
using System.Windows;

namespace S1XViewer
{
    /// <summary>
    /// Interaction logic for TidalCurveWindow.xaml
    /// </summary>
    public partial class TidalCurveWindow : Window
    {       
        public TidalCurveWindow()
        {
            InitializeComponent();

            Labels = new[] { "00:00", "01:00", "02:00", "03:00", "04:00", "05:00", "06:00", "07:00", "08:00", "09:00", "10:00", "11:00", "12:00", "13:00", "14:00", "15:00", "16:00", "17:00", "18:00", "19:00", "20:00", "21:00", "22:00", "23:00"};
            YFormatter = value => value.ToString("F01");

            DataContext = this;
        }

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }
    }
}
