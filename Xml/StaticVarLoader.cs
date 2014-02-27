using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Xml
{
    public class StaticVarLoader : ITemplateVarsLoader
    {
        private List<TemplateVariable> _vars;

        public StaticVarLoader(string filename)
        {
            _vars = new List<TemplateVariable>();
            var xml = XElement.Load(filename);

            var langs = xml.Descendants("lang");

            foreach (var lang in langs)
            {
                LoadLang(lang);
            }
        }

        public IEnumerable<TemplateVariable> GetVariables()
        {
            return _vars;
        }

        private void LoadLang(XElement xml)
        {
            var list = _vars;
            Language lang = (Language)Enum.Parse(typeof(Language), xml.Attribute("code").Value);

            foreach (var temp in xml.Elements("tplvar"))
            {
                var name = temp.Attribute("varname").Value;
                var value = temp.Value;

                var already = list.Where(var => var.Name == name);

                if (already.Any())
                {
                    var varT = already.First();
                    varT.Translates.Add(lang, value);
                }
                else
                {
                    var newVar = new TemplateVariable
                    {
                        Name = name,
                        Translates = new Dictionary<Language, string> { {lang, value} }
                    };
                    list.Add(newVar);
                }
            }
        }
    }
}
