using S1XViewer.Types;
using S1XViewer.Types.Interfaces;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System;
using System.Collections;
using System.Collections.Generic;

namespace S1XViewer
{
    /// <summary>
    /// Interaction logic for DefineColourSchemeWindow.xaml
    /// </summary>
    public partial class DefineColourSchemeWindow : Window
    {
        public ObservableCollection<ColorSchemeRangeItem> Items { get; set; }
        public IFeatureRendererManager FeatureRendererManager { get; set; }
        public string Standard { get; set;
        }
        /// <summary>
        /// 
        /// </summary>
        public DefineColourSchemeWindow()
        {
            InitializeComponent();

            this.DataContext = this;
            dataGridColorSchemes.ItemsSource = Items;
            Items = new ObservableCollection<ColorSchemeRangeItem>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonNewColorScheme_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            // save XML to color scheme file

            Close();
        }
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void buttonAddRow_Click(object sender, RoutedEventArgs e)
        {
            dataGridColorSchemes.Items.Add(new ColorSchemeRangeItem());
        }

        private void buttonRemoveRow_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        ///     Load XML and show contents in the datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxColorSchemes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            if (e is null || e.AddedItems == null || e.AddedItems.Count == 0)
            {
                throw new ArgumentNullException(nameof(e));
            }

            var selectedFileName = e.AddedItems[0].ToString();

            if (String.IsNullOrEmpty(selectedFileName) == false)
            {
                Items.Clear();
                dataGridColorSchemes.Items.Clear();
                
                if (FeatureRendererManager.LoadColorScheme(selectedFileName, Standard) == true)
                {                    
                    foreach (ColorSchemeRangeItem colorSchemeItem in FeatureRendererManager.ColorScheme)
                    {
                        dataGridColorSchemes.Items.Add(colorSchemeItem);
                    }
                }
            }            
        }
    }
}
