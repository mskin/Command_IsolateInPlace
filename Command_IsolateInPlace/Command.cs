#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;

#endregion

namespace Command_IsolateInPlace
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            ElementClassFilter filter = new ElementClassFilter(typeof(FamilyInstance));
            FilteredElementCollector collector = new FilteredElementCollector(doc, doc.ActiveView.Id);
            collector.WherePasses(filter);

            ICollection<ElementId> elementsToIsolate = new HashSet<ElementId>();

            foreach (Element el in collector)
            {
                FamilyInstance fi = el as FamilyInstance;
                Family f = fi.Symbol.Family;

                if (f.IsInPlace)
                {
                    elementsToIsolate.Add(fi.Id);
                }
            }

            Transaction trans = new Transaction(doc, "Isolate In Place Families");
            trans.Start();
            View v = doc.ActiveView;
            v.IsolateElementsTemporary(elementsToIsolate);
            trans.Commit();

            return Result.Succeeded;
        }
    }
}
