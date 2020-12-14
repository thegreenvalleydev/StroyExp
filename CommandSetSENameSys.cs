#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
#endregion

namespace StroyExp
{
    [Transaction(TransactionMode.Manual)]
    public class CommandSetSENameSys : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            //Получение списка MEP елементов
            IList<Element> mEPelements = MEPManager.GetMEPElementList(doc);    

            // Access current selection

            Selection sel = uidoc.Selection;

            // Retrieve elements from database

            FilteredElementCollector col
              = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .OfCategory(BuiltInCategory.INVALID)
                .OfClass(typeof(Wall));

            // Filtered element collector is iterable

            foreach (Element e in col)
            {
                Debug.Print(e.Name);
            }

            // Modify document within a transaction

            

            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Запись параметра СЭ_Имя системы");
                foreach (var elem in mEPelements) MEPManager.SetSESysNameParam(doc, elem);
                tx.Commit();
            }

            return Result.Succeeded;
        }
    }
}
