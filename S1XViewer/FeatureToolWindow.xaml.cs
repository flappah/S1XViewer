using S1XViewer.Base;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace S1XViewer
{
    /// <summary>
    /// Interaction logic for FeatureToolWindow.xaml
    /// </summary>
    public partial class FeatureToolWindow : Window
    {
        public string TemplateFields { get; set; } = string.Empty;
        public DataRowCollection FieldCollection { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public FeatureToolWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void labelLink_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var splittedKeyValues = TemplateFields.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (splittedKeyValues.Length > 0)
            {
                foreach (string keyValue in splittedKeyValues)
                {
                    var splittedKeyValue = keyValue.Split(new[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (splittedKeyValue.Length == 2)
                    {
                        var control = WpfExtensions.FindChild(this, splittedKeyValue[0], typeof(Label));
                        var fieldResult = FieldCollection.ContainsKey(splittedKeyValue[1]);

                        if (control != null && fieldResult.result == true)
                        {
                            ((Label)control).Content = fieldResult.value;
                        }
                    }
                }
            }
        }
    }
}
