using Autofac;
using Esri.ArcGISRuntime.Mapping;
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
        private bool _isInitialized = false;

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
                LoadBasemapStyles();
                RestoreOptions();
                _isInitialized = true;
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadBasemapStyles()
        {
            Array basemapStyles = Enum.GetValues(typeof(BasemapStyle));
            Array.Sort(basemapStyles);
            foreach (BasemapStyle basemapStyle in basemapStyles)
            {
                _syncContext.Send(o =>
                {
                    var item = (BasemapStyle)o;

                    var comboBoxItem = new ComboBoxItem
                    {
                        Content = item.ToString(),
                        IsSelected = item.ToString().Equals("ArcGISTopographic"),
                        Tag = item.ToString()
                    };

                    _ = comboBoxBasemap.Items.Add(comboBoxItem);
                }, basemapStyle);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadCRSfile()
        {
            StreamReader streamReader = null;

            try
            {
                var assemblyPath = System.Reflection.Assembly.GetAssembly(typeof(OptionsWindow)).Location;
                string folder = System.IO.Path.GetDirectoryName(assemblyPath);
                streamReader = File.OpenText($@"{folder}\crs.csv");
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
                                    IsSelected = item[0].Contains("4326"),
                                    Tag = item[0]
                                };

                                _ = comboBoxCRS.Items.Add(comboBoxItem);
                            }, splittedLine);
                        }
                    }
                }
            }
            catch (Exception) { }
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
            if (_optionsStorage != null)
            {
                _syncContext.Post(o =>
                {
                    var optionsStorage = (IOptionsStorage)o;
                    checkBoxInvertLatLon.IsChecked = _optionsStorage.Retrieve("checkBoxInvertLatLon") == "true";

                    string selectedcomboBoxCRS = _optionsStorage.Retrieve("comboBoxCRS");
                    comboBoxCRS.SelectedItem = selectedcomboBoxCRS;

                    string selectedComboBaseMap = _optionsStorage.Retrieve("comboBoxBasemap");
                    comboBoxBasemap.SelectedItem = selectedComboBaseMap;

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
            if (_isInitialized == true)
            {
                if (sender is System.Windows.Controls.ComboBox comboBoxSender)
                {
                    _optionsStorage.Store(comboBoxSender.Name, ((ComboBoxItem)e.AddedItems[0]).Tag.ToString());
                    label2.Visibility = Visibility.Visible;
                }
            }
        }

        /// <summary>
        ///     Checkbox handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            if (_isInitialized == true)
            {
                if (sender is System.Windows.Controls.CheckBox checkBoxSender)
                {
                    _optionsStorage.Store(checkBoxSender.Name, "true");
                    label2.Visibility = Visibility.Visible;
                }
            }
        }

        /// <summary>
        ///     Checkbox handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_isInitialized == true)
            {
                if (sender is System.Windows.Controls.CheckBox checkBoxSender)
                {
                    _optionsStorage.Store(checkBoxSender.Name, "false");
                    label2.Visibility = Visibility.Visible;
                }
            }
        }

        /// <summary>
        ///     Save basemap selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxBasemap_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isInitialized == true)
            {
                _optionsStorage.Store(comboBoxBasemap.Name, ((ComboBoxItem)e.AddedItems[0]).Tag.ToString());
                label2.Visibility = Visibility.Visible;
            }
        }

    }
}
