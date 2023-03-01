using Autofac;
using S1XViewer.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        private SynchronizationContext _syncContext;

        /// <summary>
        ///     For retrieving IoC objects.
        /// </summary>
        public Autofac.IContainer Container
        {
            get => _container;
            set
            {
                _container = value;
                _optionsStorage = Container.Resolve<IOptionsStorage>();
                RestoreOptions();

                comboBoxCRS.IsEnabled = true;
                checkBoxInvertLatLon.IsEnabled = true;
            }
        }

        private Autofac.IContainer _container;
        private IOptionsStorage _optionsStorage;

        /// <summary>
        ///     Options menu constructor for setup and initialization
        /// </summary>
        public OptionsWindow()
        {
            InitializeComponent();
            _syncContext = SynchronizationContext.Current;

            _ = Task.Run(() =>
            {
                LoadCRSfile();
                RestoreOptions();
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadCRSfile()
        {
            StreamReader streamReader = null;

            try
            {
                streamReader = File.OpenText("crs.csv");
                while (!streamReader.EndOfStream)
                {
                    string line = streamReader.ReadLine();
                    if (line.Contains(","))
                    {
                        string[] splittedLine = line.Split(new[] { ',' });

                        if (splittedLine.Length == 2)
                        {
                            _syncContext.Send(o =>
                            {
                                var item = (string[])o;

                                var comboBoxItem = new ComboBoxItem
                                {
                                    Content = $"EPSG:{item[0]} - {item[1]}",
                                    Tag = item[0]
                                };

                                _ = comboBoxCRS.Items.Add(comboBoxItem);
                            }, splittedLine);
                        }
                    }
                }
            }
            finally
            {
                if (streamReader != null)
                {
                    streamReader.Close();
                }
            }
        }

        /// <summary>
        ///     Restores saved option selections
        /// </summary>
        private void RestoreOptions()
        {
            if (_optionsStorage != null && _optionsStorage.Count > 0)
            {
                _syncContext.Post(o =>
                {
                    var optionsStorage = (IOptionsStorage)o;
                    checkBoxInvertLatLon.IsChecked = _optionsStorage.Retrieve("checkBoxInvertLatLon") == "true";

                    string selectedItemTag = _optionsStorage.Retrieve("comboBoxCRS");
                    foreach (object item in comboBoxCRS.Items)
                    {
                        if (item is ComboBoxItem comboBoxItem)
                        {
                            if (comboBoxItem.Tag.ToString().Equals(selectedItemTag))
                            {
                                comboBoxCRS.SelectedItem = comboBoxItem;
                                break;
                            }
                        }
                    }
                }, _optionsStorage);
            }
        }

        /// <summary>
        ///     Close button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is System.Windows.Controls.ComboBox comboBoxSender)
            {
                _optionsStorage.Store(comboBoxSender.Name, ((ComboBoxItem)e.AddedItems[0]).Tag.ToString());
            }
        }

        /// <summary>
        ///     Checkbox handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.CheckBox checkBoxSender)
            {
                _optionsStorage.Store(checkBoxSender.Name, "true");
            }
        }

        /// <summary>
        ///     Checkbox handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.CheckBox checkBoxSender)
            {
                _optionsStorage.Store(checkBoxSender.Name, "false");
            }
        }
    }
}
