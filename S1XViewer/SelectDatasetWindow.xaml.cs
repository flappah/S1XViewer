using S1XViewer.Types;
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
    /// Interaction logic for SelectDatasetWindow.xaml
    /// </summary>
    public partial class SelectDatasetWindow : Window
    {
        private string _selectedFileName = string.Empty;
        public string SelectedFilename { get { return _selectedFileName; } }

        public SelectDatasetWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Manual close w/o selecting anything
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        ///     Change selected filename
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is DatasetInfo info)
            {
                _selectedFileName = info.FileName;
            }
        }

        /// <summary>
        ///     If item is doubleclicked, close the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
