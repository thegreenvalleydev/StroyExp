using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StroyExp
{
    static class MEPManager
    {
        private static Guid SE_SYSNAME = new Guid("dcd080e0-f107-4959-be2d-05a450ce4218");

        //
        // Summary:
        //     "Запись приоритетного наименования типа системы в параметр СЭ_Имя системы"
        public static void SetSESysNameParam(Document doc, Element element)
        {
            Parameter sESysNameParam = element.get_Parameter(SE_SYSNAME);
            IList<MEPSystem> mEPSystems = GetSystemListFromElement(doc, element);
            List<string[]> names = new List<string[]>();
            string paramValue = "";

            foreach (MEPSystem mEPSystem in mEPSystems)
            {
                if (mEPSystem != null)
                {
                    MEPSystemType mEPSystemType = (MEPSystemType)doc.GetElement(mEPSystem.GetTypeId());

                    string sysDescription = mEPSystemType.get_Parameter(BuiltInParameter.ALL_MODEL_DESCRIPTION).AsString(); //Встроенный параметр Описание
                    //string sysAbbreviation = mEPSystemType.get_Parameter(BuiltInParameter.RBS_SYSTEM_ABBREVIATION_PARAM).AsString(); //Встроенный параметр Сокращение !!! Сокращение на данный момент не используется
                    Parameter sysClassKeyparam = mEPSystemType.LookupParameter("Код по классификатору");
                    //string sysClassKey = mEPSystemType.get_Parameter(BuiltInParameter.UNIFORMAT_CODE).AsString(); //Встроенный параметр Код по классификатору !!! отсутствует в категории
                    string sysClassKey = sysClassKeyparam?.AsString();
                    names.Add(new string[] { sysClassKey, sysDescription });
                }
            }

            if (names.Count > 0)
            {
                names = names.OrderBy(x => x[0]).ThenBy(x => x[1]).ToList();
                string[] name = names.First();
                paramValue = name[1];
                if (names.Count > 1 && ((BuiltInCategory)element.Category.Id.IntegerValue == BuiltInCategory.OST_PlumbingFixtures)) paramValue = "Cанитарно-технические приборы";
            }
            else
            {
                paramValue = "";
            }

            if (sESysNameParam != null && !sESysNameParam.IsReadOnly)
            {
                ParameterManager.SetParameterValue(sESysNameParam, paramValue);
            }
        }

        //
        // Summary:
        //     "Получение списка MEP элементов из документа"
        private static IList<MEPSystem> GetSystemListFromElement(Document doc, Element revitElement)
        {
            Category category = revitElement.Category;
            BuiltInCategory builtInCategory = (BuiltInCategory)category.Id.IntegerValue;
            IList<MEPSystem> mEPSystems = new List<MEPSystem>();
            foreach (MEPSystem mEPSystem in GetMepSystemFromObject(doc, revitElement, builtInCategory))
            {
                mEPSystems.Add(mEPSystem);
            }
            return mEPSystems;
        }

        //
        // Summary:
        //     "Получение списка MEP элементов из документа"
        static private IList<MEPSystem> GetMepSystemFromObject(Document doc, object obj, BuiltInCategory builtInCategory)
        {
            IList<MEPSystem> mEPSystems = new List<MEPSystem>();
            ConnectorSet connectorSet = new ConnectorSet();
            switch (builtInCategory)
            {
                case BuiltInCategory.OST_PipeAccessory: //Арматура трубопровода
                    connectorSet = (obj as FamilyInstance).MEPModel.ConnectorManager?.Connectors;
                    break;
                case BuiltInCategory.OST_PipeCurves: //Труба Pipe MEPCurve
                    connectorSet = (obj as MEPCurve).ConnectorManager.Connectors;
                    break;
                case BuiltInCategory.OST_PipeFitting: //Фитинг FamilyInstance
                    connectorSet = (obj as FamilyInstance).MEPModel.ConnectorManager?.Connectors;
                    break;
                case BuiltInCategory.OST_PipeInsulations: //Изоляция HostObject MEPCurve
                    Element elem = doc.GetElement((obj as InsulationLiningBase).HostElementId);
                    return GetMepSystemFromObject(doc, elem, (BuiltInCategory)elem.Category.Id.IntegerValue);
                case BuiltInCategory.OST_PlumbingFixtures: //Сантехника FamilyInstance
                    connectorSet = (obj as FamilyInstance).MEPModel.ConnectorManager?.Connectors;
                    break;
                case BuiltInCategory.OST_FlexPipeCurves: //Изогнутый трубопровод HostObject MEPCurve
                    connectorSet = (obj as MEPCurve).ConnectorManager.Connectors;
                    break;
                case BuiltInCategory.OST_PlaceHolderPipes: //Заполнитель трубы MEPCurve
                    connectorSet = (obj as MEPCurve).ConnectorManager.Connectors;
                    break;
                case BuiltInCategory.OST_MechanicalEquipment: //Оборудование FamilyInstance MepModel
                    connectorSet = (obj as FamilyInstance).MEPModel.ConnectorManager?.Connectors;
                    break;
                case BuiltInCategory.OST_SpecialityEquipment: //Специальное оборудование FamilyInstance MepModel
                    connectorSet = (obj as FamilyInstance).MEPModel.ConnectorManager?.Connectors;
                    break;
                case BuiltInCategory.OST_Sprinklers: //Спринклеры
                    connectorSet = (obj as FamilyInstance).MEPModel.ConnectorManager?.Connectors;
                    break;
                case BuiltInCategory.OST_DuctAccessory: //Арматура воздуховода
                    connectorSet = (obj as FamilyInstance).MEPModel.ConnectorManager?.Connectors;
                    break;
                case BuiltInCategory.OST_DuctCurves: //Воздуховоды
                    connectorSet = (obj as MEPCurve).ConnectorManager.Connectors;
                    break;
                case BuiltInCategory.OST_DuctFitting: //Фитинги воздуховодов
                    connectorSet = (obj as FamilyInstance).MEPModel.ConnectorManager?.Connectors;
                    break;
                case BuiltInCategory.OST_DuctInsulations: //Изоляция воздуховодов
                    Element elem2 = doc.GetElement((obj as InsulationLiningBase).HostElementId);
                    return GetMepSystemFromObject(doc, elem2, (BuiltInCategory)elem2.Category.Id.IntegerValue);
                case BuiltInCategory.OST_DuctTerminal: //??? Воздухораспределитель возможно
                    connectorSet = (obj as FamilyInstance).MEPModel.ConnectorManager?.Connectors;
                    break;
                case BuiltInCategory.OST_FlexDuctCurves: //Изогнутый воздуховод
                    connectorSet = (obj as MEPCurve).ConnectorManager.Connectors;
                    break;
                case BuiltInCategory.OST_PlaceHolderDucts: //Заполнитель воздуховода
                    connectorSet = (obj as MEPCurve).ConnectorManager.Connectors;
                    break;

            }
            if (connectorSet != null)
            {
                foreach (Connector connector in connectorSet)
                    mEPSystems.Add(connector.MEPSystem);
            }
            return mEPSystems;
        }

        //
        // Summary:
        //     "Получение списка MEP элементов из документа"
        public static IList<Element> GetMEPElementList(Document doc)
        {
            IList<Element> outElemList = new List<Element>();
            foreach (BuiltInCategory builtInCategory in GetMEPCategories())
            {
                IList<Element> elemList = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .OfCategory(builtInCategory)
                .ToElements();
                outElemList = outElemList.Union(elemList).ToList<Element>();
            }
            return outElemList;
        }

        //
        // Summary:
        //     "Список MEP категорий"
        public static IList<BuiltInCategory> GetMEPCategories()
        {
            IList<BuiltInCategory> cats = new List<BuiltInCategory>();
            cats = cats.Union(GetPipeCategories()).ToList<BuiltInCategory>();
            cats = cats.Union(GetDuctCategories()).ToList<BuiltInCategory>();
            return cats;
        }

        //
        // Summary:
        //     "Список категорий вентиляции"
        public static IList<BuiltInCategory> GetDuctCategories()
        {
            IList<BuiltInCategory> cats = new List<BuiltInCategory>();
            cats.Add(BuiltInCategory.OST_DuctAccessory); //Арматура воздуховодов FamilyInstance MepModel
            cats.Add(BuiltInCategory.OST_DuctCurves); //Воздуховод HostObject MEPCurve
            cats.Add(BuiltInCategory.OST_DuctFitting);
            cats.Add(BuiltInCategory.OST_DuctInsulations);
            cats.Add(BuiltInCategory.OST_DuctTerminal);
            cats.Add(BuiltInCategory.OST_FlexDuctCurves);
            cats.Add(BuiltInCategory.OST_PlaceHolderDucts);
            cats.Add(BuiltInCategory.OST_SpecialityEquipment);
            cats.Add(BuiltInCategory.OST_MechanicalEquipment);
            return cats;
        }

        //
        // Summary:
        //     "Список категорий трубопроводов"
        public static IList<BuiltInCategory> GetPipeCategories()
        {
            IList<BuiltInCategory> cats = new List<BuiltInCategory>();
            cats.Add(BuiltInCategory.OST_PipeAccessory); //Арматура трубопровода FamilyInstance MepModel
            cats.Add(BuiltInCategory.OST_PipeCurves); //Труба Pipe MEPCurve
            cats.Add(BuiltInCategory.OST_PipeFitting); //Фитинг FamilyInstance
            cats.Add(BuiltInCategory.OST_PipeInsulations); //Изоляция HostObject MEPCurve
            cats.Add(BuiltInCategory.OST_PlumbingFixtures); //Сантехника FamilyInstance
            cats.Add(BuiltInCategory.OST_FlexPipeCurves); //Изогнутый трубопровод HostObject MEPCurve
            cats.Add(BuiltInCategory.OST_PlaceHolderPipes); //Заполнитель трубы MEPCurve
            cats.Add(BuiltInCategory.OST_MechanicalEquipment); //Оборудование FamilyInstance MepModel
            cats.Add(BuiltInCategory.OST_SpecialityEquipment); //Специальное оборудование FamilyInstance MepModel
            cats.Add(BuiltInCategory.OST_Sprinklers); //Спринклеры FamilyInstance MepModel
            return cats;
        }
    }
}
