using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;

namespace Xml
{
    public class ToXmlConverter : IToXmlConverter
    {
        private IEnumerable<TemplateVariable> _variables;

       
        public ToXmlConverter(
            ITemplateVarsLoader varLoader
            )
        {
            _variables = varLoader.GetVariables();

        }

        public System.Xml.Linq.XElement GetFromNative(string filename)
        {
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException(filename);
            }

            var result = new XElement(XmlDataValues.RootTitle);



            return result;
        }

        private XElement ListVariables()
        {
            var result = new XElement(XmlDataValues.TVariablesTitle);

            result.Add(_variables.GetXmlForLanguage(Language.Jap));
            result.Add(_variables.GetXmlForLanguage(Language.Eng));
            result.Add(_variables.GetXmlForLanguage(Language.Rus));

            return result;
        }
    }
}
