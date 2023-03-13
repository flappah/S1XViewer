using Esri.ArcGISRuntime.Tasks.NetworkAnalysis;
using System;
using System.Windows;
using System.Windows.Controls;

namespace S1XViewer
{
    /// <summary>
    /// Interaction logic for SelectDateTimeWindow.xaml
    /// </summary>
    public partial class SelectDateTimeWindow : Window
    {
        private static DateTime? _selectedDateTime = null;
        private DateTime? _firstValidDate = null;
        private DateTime? _lastValidDate = null;
        private bool _initialized = false;

        /// <summary>
        ///     SelectedDateTime
        /// </summary>
        public DateTime SelectedDateTime { get { return (_selectedDateTime ?? DateTime.Now); } }

        /// <summary>
        ///     FirstValidDate
        /// </summary>
        public DateTime FirstValidDate
        {
            get { return (DateTime)(_firstValidDate ?? DateTime.MinValue); } 
            set { 
                _firstValidDate = value;
                datePicker.DisplayDateStart = _firstValidDate;
            }
        }

        /// <summary>
        ///     LastValidDate
        /// </summary>
        public DateTime LastValidDate
        {
            get { return (DateTime)(_lastValidDate ?? DateTime.MaxValue); }
            set
            {
                _lastValidDate = value;
                datePicker.DisplayDateEnd = _lastValidDate;
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public SelectDateTimeWindow()
        {
            InitializeComponent();

            if (_selectedDateTime != null)
            {
                datePicker.SelectedDate = _selectedDateTime;
                timePicker.Text = ((DateTime)_selectedDateTime).ToString("HH:mm");
            }
            else
            {
                timePicker.SelectedIndex = 0;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            DateTime selectedDateTimeFromUI = (DateTime) datePicker.SelectedDate;
            var selectedTime = ((ComboBoxItem)timePicker.SelectedItem).Content.ToString();
            if (DateTime.TryParse(selectedDateTimeFromUI.ToString("yyyy-MM-dd") + " " + selectedTime, 
                out DateTime selectedDateTime) == true)
            {
                _selectedDateTime = selectedDateTime;  
            }

            this.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void datePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_initialized == true)
            {
                DateTime selectedDateTimeFromUI = (DateTime)datePicker.SelectedDate;
                if (DateTime.TryParse(selectedDateTimeFromUI.ToString("yyyy-MM-dd") + " " + SelectedDateTime.ToString("HH:mm"),
                    out DateTime selectedDateTime) == true)
                {
                    if (selectedDateTime >= FirstValidDate && selectedDateTime <= LastValidDate)
                    {
                        _selectedDateTime = selectedDateTime;
                    }
                    else
                    {
                        _initialized = false;
                        datePicker.DisplayDateStart = _firstValidDate;
                        datePicker.DisplayDateEnd = _lastValidDate;
                        datePicker.SelectedDate = _firstValidDate;
                        _initialized = true;

                        MessageBox.Show($"Selected date is outside the valid timeframe! Select a date between {FirstValidDate.ToString("yyyy-MM-dd")} and {LastValidDate.ToString("yyyy-MM-dd")}.");
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (timePicker.SelectedItem != null)
            {
                var selectedTime = ((ComboBoxItem)timePicker.SelectedItem).Content.ToString();
                if (DateTime.TryParse(SelectedDateTime.ToString("yyyy-MM-dd") + " " + selectedTime,
                   out DateTime selectedDateTime) == true)
                {
                    _selectedDateTime = selectedDateTime;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _initialized = true;
        }
    }
}
