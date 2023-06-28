using LiveCharts;
using LiveCharts.Wpf;
using S1XViewer.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace S1XViewer
{
    /// <summary>
    /// Interaction logic for FeatureToolWindow.xaml
    /// </summary>
    public partial class FeatureToolWindow : Window
    {
        public string TemplateFields { get; set; } = string.Empty;
        public DataRowCollection FieldCollection { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public FeatureToolWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void labelLink_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var series = new ChartValues<double>();
            var xAsis = new List<DateTime>();

            var stationName = string.Empty;
            foreach (DataRow row in FieldCollection)
            {
                if (row != null && row.ItemArray.Length > 1)
                {
                    if (row.ItemArray[0].ToString().Equals("TidalHeights"))
                    {
                        var data = row.ItemArray[1].ToString();
                        var splittedDataValues = data.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (string splittedValue in splittedDataValues)
                        {
                            var keyValue = splittedValue.Split(new[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                            if (keyValue.Length == 2)
                            {
                                if (DateTime.TryParseExact(keyValue[0], "ddMMMyyyy HHss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime timeStamp))
                                {
                                    xAsis.Add(timeStamp);
                                }

                                if (double.TryParse(keyValue[1].Replace(",", "."), System.Globalization.NumberStyles.Float, new CultureInfo("en-US"), out double height))
                                {
                                    series.Add(height);
                                }
                            }
                        }
                    }

                    if (row.ItemArray[0].ToString().Equals("FeatureName.DisplayName"))
                    {
                        stationName = row.ItemArray[1].ToString();
                    }
                }
            }

            var tidalCurveWindow = new TidalCurveWindow();
            tidalCurveWindow.Title = "Tidal Curve at " + stationName;
            tidalCurveWindow.SeriesCollection = new SeriesCollection() 
            {
                new LineSeries
                {
                    Title = "Tidal Heights",
                    Values = series,
                    LineSmoothness = 1,
                    PointGeometry = null
                } 
            };
            tidalCurveWindow.ShowDialog();
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var splittedKeyValues = TemplateFields.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (splittedKeyValues.Length > 0)
            {
                foreach (string keyValue in splittedKeyValues)
                {
                    var splittedKeyValue = keyValue.Split(new[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (splittedKeyValue.Length == 2)
                    {
                        var control = WpfExtensions.FindChild(this, splittedKeyValue[0], typeof(Label));
                        var fieldResult = FieldCollection.ContainsKey(splittedKeyValue[1]);

                        if (control != null && fieldResult.result == true)
                        {
                            ((Label)control).Content = fieldResult.value;
                        }
                    }
                }
            }
        }
    }
}
