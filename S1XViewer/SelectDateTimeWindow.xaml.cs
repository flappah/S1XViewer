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
        private DateTime _selectedDateTime;
        public DateTime SelectedDateTime { get { return _selectedDateTime; } }

        public SelectDateTimeWindow()
        {
            InitializeComponent();
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
            DateTime selectedDateTimeFromUI = (DateTime)datePicker.SelectedDate;
            if (DateTime.TryParse(selectedDateTimeFromUI.ToString("yyyy-MM-dd") + " " + SelectedDateTime.ToString("HH:mm"),
                out DateTime selectedDateTime) == true)
            {
                _selectedDateTime = selectedDateTime;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedTime = ((ComboBoxItem)timePicker.SelectedItem).Content.ToString();
            if (DateTime.TryParse(SelectedDateTime.ToString("yyyy-MM-dd") + " " + selectedTime,
               out DateTime selectedDateTime) == true)
            {
                _selectedDateTime = selectedDateTime;
            }
        }
    }
}
