using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Xml
{
    public enum Language
    {
        Eng,
        Jap,
        Rus,
        NotSpecified
    }

    public class TemplateVariable
    {
        public string Name { get; set; }

        public Dictionary<Language, string> Translates { get; set; }

    }

   
}
