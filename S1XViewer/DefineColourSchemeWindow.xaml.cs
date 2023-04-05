﻿using S1XViewer.Base;
using S1XViewer.Types;
using S1XViewer.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace S1XViewer
{
    /// <summary>
    /// Interaction logic for DefineColourSchemeWindow.xaml
    /// </summary>
    public partial class DefineColourSchemeWindow : Window
    {
        private bool _isChanged = false;

        //public ObservableCollection<ColorSchemeRangeItem> Items { get; set; } 
        public IFeatureRendererManager? FeatureRendererManager { get; set; }
        public string Standard { get; set; } = string.Empty;
        public string ColorSchemeName { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public DefineColourSchemeWindow()
        {
            InitializeComponent();

            //this.DataContext = this;
            //dataGridColorSchemes.ItemsSource = Items;
            //Items = new ObservableCollection<ColorSchemeRangeItem>();
        }

        /// <summary>
        ///     Handles saving of colorschemes and closes window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            // save XML to color scheme file
            MessageBoxResult messageBoxResult = MessageBoxResult.OK;

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

                foreach (var item in dataGridColorSchemes.Items)
                {
                    if (item is ColorSchemeRangeItem colorSchemeRangeItem)
                        colorSchemeRangeItem.WriteXml(xmlWriter);
                }

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();

                xmlWriter.Flush();
                xmlWriter.Close();

                string xmlString = stringWriter.ToString();
                // write to disk
                var fullPath = System.Reflection.Assembly.GetAssembly(GetType())?.Location;
                var directory = Path.GetDirectoryName(fullPath);

                if (File.Exists(@$"{directory}\colorschemes\{comboBoxColorSchemes.Text}"))
                {
                    messageBoxResult = MessageBox.Show("Color scheme exists! Do you want to overwrite the color scheme?", "Color scheme exists", MessageBoxButton.YesNoCancel);
                }

                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    File.WriteAllText(@$"{directory}\colorschemes\{comboBoxColorSchemes.Text}", xmlString);
                }
            }

            if (messageBoxResult != MessageBoxResult.Cancel)
            {
                Close();
            }
        }

        /// <summary>
        ///     Cancel just closes the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBoxResult.Yes;
            if (_isChanged == true)
            {
                messageBoxResult = MessageBox.Show("Changes have been made. Are you sure to cancel the changes?", "Changes have been made", MessageBoxButton.YesNo);
            }

            if (messageBoxResult == MessageBoxResult.Yes)
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
            if (dataGridColorSchemes.SelectedItems != null && dataGridColorSchemes.SelectedItems.Count > 0)
            {
                var selectedItem = dataGridColorSchemes.SelectedItems[0] as ColorSchemeRangeItem;
                if (selectedItem != null)
                {
                    var copiedItems = new List<ColorSchemeRangeItem>();
                    foreach (ColorSchemeRangeItem item in dataGridColorSchemes.Items)
                    {
                        copiedItems.Add(item);
                    }
                    dataGridColorSchemes.Items.Clear();

                    var newItems = new List<ColorSchemeRangeItem>();
                    int i = 0;
                    var selectedId = selectedItem.Id;
                    foreach (ColorSchemeRangeItem item in copiedItems)
                    {
                        if (item.Id == selectedId)
                        {
                            newItems.Add(new ColorSchemeRangeItem() { Id = i++ });
                        }

                        item.Id = i++;
                        newItems.Add(item);
                    }

                    foreach (var item in newItems)
                    {
                        dataGridColorSchemes.Items.Add(item);
                    }

                    dataGridColorSchemes.Items.Refresh();
                }
            }
            else
            {
                int highestId = dataGridColorSchemes.Items.Count == 0 ? -1 : ((ColorSchemeRangeItem)dataGridColorSchemes.Items[dataGridColorSchemes.Items.Count - 1]).Id;
                dataGridColorSchemes.Items.Add(new ColorSchemeRangeItem() { Id = highestId + 1 });
            }

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

                if (selectedRow != null)
                {
                    int i = 0;
                    foreach (ColorSchemeRangeItem item in dataGridColorSchemes.Items)
                    {
                        if (item.Id == selectedRow.Id)
                        {
                            break;
                        }
                        i++;
                    }

                    if (i > 0)
                    {
                        ((ColorSchemeRangeItem)dataGridColorSchemes.Items[i - 1]).Max = selectedRow.Max;
                    }
                    else
                    {
                        ((ColorSchemeRangeItem)dataGridColorSchemes.Items[i + 1]).Min = selectedRow.Min;
                    }

                    dataGridColorSchemes.Items.Remove(selectedRow);

                    _isChanged = true;
                    if (Title.Contains("*") == false)
                    {
                        Title += "*";
                    }

                    dataGridColorSchemes.Items.Refresh();
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
                    //Items.Clear();
                    dataGridColorSchemes.Items.Clear();

                    if (FeatureRendererManager.LoadColorScheme(selectedFileName, Standard) == true)
                    {
                        int i = 0;
                        foreach (ColorSchemeRangeItem colorSchemeItem in FeatureRendererManager.ColorScheme)
                        {
                            colorSchemeItem.Id = i++;
                            dataGridColorSchemes.Items.Add(colorSchemeItem);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     if cell contents change, set window to changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridColorSchemes_CurrentCellChanged(object sender, EventArgs e)
        {
            _isChanged = true;
            if (Title.Contains("*") == false)
            {
                Title += "*";
            }
        }

        /// <summary>
        ///     To avoid 'EditItem' exception
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridColorSchemes_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true;
        }
    }
}