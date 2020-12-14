using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StroyExp.Resources.Tools
{
    static class ConfigReader
    {
        //Путь к фалу конфигурации
        public static string configFilePath = Path.Combine(Assembly.GetExecutingAssembly().Location, "config.ini");

    }
}
