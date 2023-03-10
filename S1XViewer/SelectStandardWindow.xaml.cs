using System.Windows;
using System.Windows.Controls;

namespace S1XViewer
{
    /// <summary>
    /// Interaction logic for SelectStandard.xaml
    /// </summary>
    public partial class SelectStandardWindow : Window
    {
        private string _selectedStandard = string.Empty;
        public string SelectedStandard { get { return _selectedStandard; } }

        public SelectStandardWindow()
        {
            InitializeComponent();
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedStandard = ((ComboBoxItem) e.AddedItems[0]).Content.ToString();
        }
    }
}
