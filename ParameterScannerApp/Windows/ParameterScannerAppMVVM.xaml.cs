using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
//using ParameterScannerApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows;

namespace ParameterScannerApp
{
    public partial class ParameterScannerAppMVVM : Window, IDisposable
    {
        public static List<ElementId> matchingElementsIds;
        public static string parameterName { get; set; }
        public static string parameterValue { get; set; }

        public static ParameterScannerAppMVVM MainView { get; set; }
        public static ExternalEvent SelectEvent { get; set; } = ExternalEvent.Create(new SelectElementsByParameterAndValueEEH(matchingElementsIds));
        public static ExternalEvent IsolateEvent { get; set; } = ExternalEvent.Create(new IsolateElementsByElementIdEEH(matchingElementsIds));
        
        public ParameterScannerAppMVVM()
        {
            MainView = this;
            InitializeComponent();
        }
        private void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
        public void Select_Button_Click(object sender, RoutedEventArgs e)
        {
            SelectEvent.Raise();
        }
        public void Isolate_Button_Click(object sender, RoutedEventArgs e)
        {
            SelectEvent.Raise();
            IsolateEvent.Raise();
        }

        private void ParameterNameTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string currentText = ParameterNameTextBox.Text;
            parameterName = currentText;
        }
        private void ParameterValueTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string currentText = ParameterValueTextBox.Text;
            parameterValue = currentText;
        }
    }
}