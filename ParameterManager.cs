using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StroyExp
{
    static class ParameterManager
    {
        public static void SetParameterValue(Parameter p, object value)
        {
            try
            {
                if (value.GetType().Equals(typeof(string)))
                {
                    if (p.SetValueString(value as string))
                        return;
                }

                switch (p.StorageType)
                {
                    case StorageType.None:
                        break;
                    case StorageType.Double:
                        if (value.GetType().Equals(typeof(string)))
                        {
                            p.Set(double.Parse(value as string));
                        }
                        else
                        {
                            p.Set(Convert.ToDouble(value));
                        }
                        break;
                    case StorageType.Integer:
                        if (value.GetType().Equals(typeof(string)))
                        {
                            p.Set(int.Parse(value as string));
                        }
                        else
                        {
                            p.Set(Convert.ToInt32(value));
                        }
                        break;
                    case StorageType.ElementId:
                        if (value.GetType().Equals(typeof(ElementId)))
                        {
                            p.Set(value as ElementId);
                        }
                        else if (value.GetType().Equals(typeof(string)))
                        {
                            p.Set(new ElementId(int.Parse(value as string)));
                        }
                        else
                        {
                            p.Set(new ElementId(Convert.ToInt32(value)));
                        }
                        break;
                    case StorageType.String:
                        p.Set(value.ToString());
                        break;
                }
            }
            catch
            {
                Debug.Print(p.StorageType.ToString() + p.Element.Name.ToString() + p.Id.ToString()); 
                throw new Exception("Invalid Value Input!");
            }
        }

        public static string GetParameterValue(Parameter p)
        {
            switch (p.StorageType)
            {
                case StorageType.Double:
                    //get value with unit, AsDouble() can get value without unit
                    return p.AsValueString();
                case StorageType.ElementId:
                    return p.AsElementId().IntegerValue.ToString();
                case StorageType.Integer:
                    //get value wwith unot, AsInteger can get value without unit
                    return p.AsValueString();
                case StorageType.None:
                    return p.AsValueString();
                case StorageType.String:
                    return p.AsString();
                default:
                    return "";

            }
        }
    }
}
