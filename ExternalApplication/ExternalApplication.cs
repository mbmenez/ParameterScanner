using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Reflection;
using System.Windows.Media.Imaging;
using ParameterScannerResources;
using System.Diagnostics;


namespace ParameterScannerApp

{
    public class ExternalApplication : IExternalApplication
    {
        public ExternalApplication() { }
        public static string AutodeskUserId { get; set; } = null;
        public static ExternalApplication parameterScannerApp = new ExternalApplication();
        internal UIApplication uiApp = null;
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
        public Result OnStartup(UIControlledApplication application)
        {
            AddRibbonButtons(application);

            return Result.Succeeded;
        }
        private void AddRibbonButtons(UIControlledApplication application)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string executingAssemblyPath = assembly.Location;
            Debug.Print(executingAssemblyPath);
            string executingAssemblyName = assembly.GetName().Name;
            Console.WriteLine(executingAssemblyName);
            string tabName = "Parameters";

            try { application.CreateRibbonTab(tabName); } catch (Exception) { }

            RibbonPanel dataPanel = application.CreateRibbonPanel(tabName, "Parameter");

            BitmapImage bitmapImage = ResourceImage.GetIcon("icons8-parameter-25.png");

            PushButtonData parameterScannerPBD = new PushButtonData("ParameterScanenr", "Parameter\nScanner", executingAssemblyPath, typeof(ParameterScannerEC).FullName)
            {
                ToolTip = "Extracts rooms data and save as a .csv file.",
                LargeImage = bitmapImage
            };
            dataPanel.AddItem(parameterScannerPBD);
        }

        internal ParameterScannerAppMVVM parameterScannerAppMVVM = null;
        internal void ShowParameterScannerAppUI()
        {
            if (parameterScannerAppMVVM == null || parameterScannerAppMVVM.IsLoaded == false)
            {
                UIDocument uidoc = uiApp.ActiveUIDocument;
                Document doc = uidoc.Document;
                parameterScannerAppMVVM = new ParameterScannerAppMVVM();
                parameterScannerAppMVVM.Show();
            }
        }
    }
}
