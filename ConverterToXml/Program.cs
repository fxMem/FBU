using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

using Xml;

namespace ConverterToXml
{
    class Program
    {
        static void Main(string[] args)
        {


            var filepath = @"C:\homeworks\fbu\script\";
            var varsFile = @"vars.xml";

            var varLoader = new StaticVarLoader(varsFile);

            var loader = new FbuDataLoader(varLoader);

            int id = 1;
            foreach (var file in Directory.EnumerateFiles(filepath))
            {
                var chapter = loader.LoadFromNative(file, id);
                id += chapter.EntryCount;

                chapter.SaveToFile(Path.ChangeExtension(file, "xml"));
            }

            

            
        }
    }
}
