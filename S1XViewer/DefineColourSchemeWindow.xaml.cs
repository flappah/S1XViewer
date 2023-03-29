using S1XViewer.Types;
using S1XViewer.Types.Interfaces;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using S1XViewer.Base;
using System.IO;
using System.DirectoryServices.ActiveDirectory;

namespace S1XViewer
{
    /// <summary>
    /// Interaction logic for DefineColourSchemeWindow.xaml
    /// </summary>
    public partial class DefineColourSchemeWindow : Window
    {
        private bool _isChanged = false;

        public ObservableCollection<ColorSchemeRangeItem> Items { get; set; } 
        public IFeatureRendererManager? FeatureRendererManager { get; set; }
        public string Standard { get; set; } = string.Empty;
        public string ColorSchemeName { get; set; } = string.Empty;

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
        ///     Handles saving of colorschemes and closes window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            // save XML to color scheme file
            if (_isChanged == true)
            {
                var stringWriter = new StringWriterWithEncoding(Encoding.UTF8);

                var xmlWriter = XmlWriter.Create(stringWriter,
                    new XmlWriterSettings
                    {
                        Encoding = Encoding.UTF8,
                        Indent = true,
                        OmitXmlDeclaration = false,
                        NewLineHandling = NewLineHandling.None
                    });

                xmlWriter.WriteStartElement("ColorSchemes");
                xmlWriter.WriteStartElement("ColorScheme");
                xmlWriter.WriteAttributeString("type", Standard);
                xmlWriter.WriteAttributeString("name", comboBoxColorSchemes.Text);

                foreach (ColorSchemeRangeItem item in dataGridColorSchemes.Items)
                {
                    item.WriteXml(xmlWriter);
                }

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();

                xmlWriter.Flush();
                xmlWriter.Close();

                string xmlString = stringWriter.ToString();
                // write to disk
                var fullPath = System.Reflection.Assembly.GetAssembly(GetType())?.Location;
                var directory = Path.GetDirectoryName(fullPath);

                MessageBoxResult messageBoxResult = MessageBoxResult.OK;
                if (File.Exists(@$"{directory}\colorschemes\{comboBoxColorSchemes.Text}"))
                {
                    messageBoxResult = MessageBox.Show("Color scheme exists! Do you want to overwrite the color scheme?", "Color scheme exists", MessageBoxButton.OKCancel);
                }

                if (messageBoxResult == MessageBoxResult.OK)
                {
                    File.WriteAllText(@$"{directory}\colorschemes\{comboBoxColorSchemes.Text}", xmlString);
                }
            }

            Close();
        }

        /// <summary>
        ///     Cancel just closes the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBoxResult.OK;
            if (_isChanged == true)
            {
                messageBoxResult = MessageBox.Show("Changes have been made. Are you sure to cancel the changes?", "Changes have been made", MessageBoxButton.OKCancel);
            }

            if (messageBoxResult == MessageBoxResult.OK)
            {
                Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAddRow_Click(object sender, RoutedEventArgs e)
        {
            dataGridColorSchemes.Items.Add(new ColorSchemeRangeItem());

            _isChanged = true;
            if (Title.Contains("*") == false)
            {
                Title += "*";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRemoveRow_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridColorSchemes.SelectedItems != null && dataGridColorSchemes.SelectedItems.Count > 0)
            {
                ColorSchemeRangeItem selectedRow = (ColorSchemeRangeItem)dataGridColorSchemes.SelectedItems[0];
                dataGridColorSchemes.Items.Remove(selectedRow);

                _isChanged = true;
                if (Title.Contains("*") == false)
                {
                    Title += "*";
                }
            }
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

            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
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
}