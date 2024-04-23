using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UIFramework.PropertyGrid;

namespace ParameterScannerApp
{

    public class SelectElementsByParameterAndValueEEH : IExternalEventHandler
    {
        private List<ElementId> matchingElementsIds;

        public SelectElementsByParameterAndValueEEH(List<ElementId> matchingElementsIds)
        {
            this.matchingElementsIds = matchingElementsIds;
        }
        public void Execute(UIApplication app)
        {
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;

            //Checks whether the active view is either a Floor Plan, Reflected Ceiling Plan, or 3D view
            ElementId activeViewId = doc.ActiveView.Id;
            List<ViewType> allowedViewsViewTypes = new List<ViewType>()
                    {
                        ViewType.FloorPlan,
                        ViewType.CeilingPlan,
                        ViewType.ThreeD
                    };
            List<ElementId> allowedDocumentViews = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Views)
                .Cast<View>()
                .Where(v => allowedViewsViewTypes.Contains(v.ViewType) == true)
                .Select(v => v.Id)
                .ToList();
            if (allowedDocumentViews.Contains(activeViewId))
            {

                FilteredElementCollector collector = new FilteredElementCollector(doc, activeViewId);
                ICollection<ElementId> modelInstancesIds = collector
                    .WhereElementIsNotElementType()
                    .WhereElementIsViewIndependent()
                    .ToElementIds();

                if (modelInstancesIds.Count > 0)
                {

                    string inputParamName = ParameterScannerAppMVVM.parameterName;
                    string inputParamValue = ParameterScannerAppMVVM.parameterValue;
                    //Stores parameter value from all elements with given parameter name
                    Dictionary<ElementId, String> elementIdAndValueString = new Dictionary<ElementId, String>();
                    List<ElementId> matchingElementsIds = new List<ElementId>();
                    foreach (ElementId instanceId in modelInstancesIds)
                    {
                        Element element = doc.GetElement(instanceId);
                        //Ensures that 'inputParamName' has a value
                        if (inputParamName != null)
                        {
                            //Searches for the given parameter within all model Family Instances
                            Parameter parameter = element.LookupParameter(inputParamName);
                            if (parameter != null)
                            {
                                StorageType parameterStorageType = parameter.StorageType;
                                if (parameterStorageType == StorageType.Integer)
                                {
                                    int intValue = element.LookupParameter(inputParamName).AsInteger();
                                    elementIdAndValueString.Add(instanceId, intValue.ToString());
                                }
                                if (parameterStorageType == StorageType.String)
                                {
                                    string stringValue = element.LookupParameter(inputParamName).AsString();
                                    elementIdAndValueString.Add(instanceId, stringValue);
                                }
                                if (parameterStorageType == StorageType.ElementId)
                                {
                                    ElementId elementIdValue = element.LookupParameter(inputParamName).AsElementId();
                                    elementIdAndValueString.Add(instanceId, elementIdValue.ToString());
                                }
                                if (parameterStorageType == StorageType.Double)
                                {
                                    double doubleIdValue = element.LookupParameter(inputParamName).AsDouble();
                                    elementIdAndValueString.Add(instanceId, doubleIdValue.ToString());
                                }
                            }
                        }
                        else
                        {
                            TaskDialog.Show("Parameter name must have value!", "Please inform the name of the parameter you're looking for.");
                        }
                    }
                    //Searches for and registers the given parameter value within the dictionary
                    if (elementIdAndValueString.Values.Contains(inputParamValue))
                    {
                        for (int i = 0; i < elementIdAndValueString.Values.Count; i++)
                        {
                            if (elementIdAndValueString.Values.ElementAt(i) == inputParamValue)
                            {
                                matchingElementsIds.Add(elementIdAndValueString.Keys.ElementAt(i));
                            }
                        }
                    }
                    if (matchingElementsIds.Any())
                    {

                        uidoc.Selection.SetElementIds(matchingElementsIds);
                        TaskDialog.Show($@"{matchingElementsIds.Count} elements found!", $@"{matchingElementsIds.Count} match input parameter and value.");
                        IsolateElementsByElementIdEEH isolateHandler = new IsolateElementsByElementIdEEH(matchingElementsIds);
                        ExternalEvent exEvent = ExternalEvent.Create(isolateHandler);
                        exEvent.Raise();
                    }
                    else
                    {
                        TaskDialog.Show("No elements were found!", "No elements match the given parameter value.");
                    }
                }
                else
                {
                    TaskDialog.Show("No elements found!", "This model has no suitable elements for parameter scanning.");
                }
            }
            else
            {
                TaskDialog.Show("View not suitable for parameter scanning!", "Please execute it in either a Floor Plan, Reflected Ceiling Plan, or 3D view.");
            }
        }
        public string GetName()
        {
            return this.GetType().Name;
        }

    }

    public class IsolateElementsByElementIdEEH : IExternalEventHandler
    {
        private List<ElementId> matchingElementsIds;

        public IsolateElementsByElementIdEEH(List<ElementId> matchingElementsIds)
        {
            this.matchingElementsIds = matchingElementsIds;
        }
        public void Execute(UIApplication app)
        {
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;
            View activeView = doc.ActiveView;
            if (matchingElementsIds != null)
            {
                if (matchingElementsIds.Count() > 0)
                {
                    Transaction tx = new Transaction(doc, "Isolating Elements");
                    tx.Start();
                    activeView.IsolateElementsTemporary(matchingElementsIds);
                    tx.Commit();
                }
                else
                {
                    TaskDialog.Show("Elements couldn't be isolated!", "There were no matching elements to isolate.");
                }
            }
        }
        public string GetName()
        {
            return this.GetType().Name;
        }
    }
}
