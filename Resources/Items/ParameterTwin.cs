using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StroyExp.Resources.Items
{
    class ParameterTwin
    {
        
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public bool IsShared { get; set; }
        public ParameterTwin(Parameter parameter)
        {

        }
    }
}
