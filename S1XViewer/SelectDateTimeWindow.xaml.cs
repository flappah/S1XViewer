using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace S1XViewer
{
    /// <summary>
    /// Interaction logic for SelectDateTimeWindow.xaml
    /// </summary>
    public partial class SelectDateTimeWindow : Window
    {
        public DateTime? SelectedDateTime { get; set; }

        public SelectDateTimeWindow()
        {
            InitializeComponent();
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            SelectedDateTime = datePicker.SelectedDate;
            this.Close();
        }
    }
}
