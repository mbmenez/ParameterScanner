using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;
using ParameterScannerApp;
using System.Collections.Generic;
using System.Linq;
using System;
using static ParameterScannerApp.ExternalApplication; 
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI.Events;

namespace ParameterScannerApp
{
    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    public class ParameterScannerEC : IExternalCommand
    {
        //public static ParameterScannerAppMVVM ParameterScannerAppMVVM { get; set; } = new ParameterScannerAppMVVM();
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                parameterScannerApp.uiApp = uiapp;
                parameterScannerApp.ShowParameterScannerAppUI();
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message + "\n\n" + ex.StackTrace;
                return Result.Failed;
            }
        }
    }
}
