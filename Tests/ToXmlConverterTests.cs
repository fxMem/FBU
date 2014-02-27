using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Xml;

namespace Tests
{
    [TestClass]
    public class ToXmlConverterTests
    {
        [TestMethod]
        public void TestTemplateVariablesXml()
        {
            var vars = new List<TemplateVariable>();
            vars.Add(new TemplateVariable { Name = "TEST1", Translates = 
            { 
                { Language.Rus, "Тест" }, 
                { Language.Eng, "Test" },
                {Language.Jap, "Fufu"}
            } });

            vars.Add(new TemplateVariable
            {
                Name = "TEST2",
                Translates = 
            { 
                { Language.Rus, "Тест2" }, 
                { Language.Eng, "Test2" },
                {Language.Jap, "Fufu2"}
            }
            } );


        }
    }
}
