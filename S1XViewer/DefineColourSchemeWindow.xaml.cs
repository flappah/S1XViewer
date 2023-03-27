using S1XViewer.Types;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace S1XViewer
{
    /// <summary>
    /// Interaction logic for DefineColourSchemeWindow.xaml
    /// </summary>
    public partial class DefineColourSchemeWindow : Window
    {
        public ObservableCollection<ColorSchemeRangeItem> Items { get; set; } = new ObservableCollection<ColorSchemeRangeItem>();

        public DefineColourSchemeWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            dataGridColorSchemes.ItemsSource = Items;
        }

        private void buttonAddColorSCheme_Click(object sender, RoutedEventArgs e)
        {
            Items.Add(new ColorSchemeRangeItem());
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
