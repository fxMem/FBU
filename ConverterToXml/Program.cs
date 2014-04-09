using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

using Xml;
using Xml.Compression;

namespace ConverterToXml
{
    class Program
    {
        static void Main(string[] args)
        {


            var filepath = @"C:\homeworks\fbu\script\";
            var outDir = @"C:\homeworks\fbu\script\out";
            var varsFile = @"vars.xml";

            if (args.Length != 3)
            {
                Console.WriteLine("You can specify arguments:");
                Console.WriteLine("1 - Folder with native script files");
                Console.WriteLine("2 - Folder for output");
                Console.WriteLine("3 - Path to variables xml file");
                Console.WriteLine("Press any key to continue with default params..");
                Console.ReadKey();
            }
            else
            {
                filepath = args[0];
                outDir = args[1];
                varsFile = args[2];

                Console.WriteLine("You have specified arguments:");
                Console.WriteLine("{0} - Folder with native script files", filepath);
                Console.WriteLine("{0} - Folder for output", outDir);
                Console.WriteLine("{0} - Path to variables xml file", varsFile);
                Console.WriteLine("Press any key to continue ..");
                Console.ReadKey();
            }

            if (!File.Exists(varsFile))
            {
                Console.WriteLine("Cannot find {0} file!", varsFile);
                return;
            }

            var varLoader = new StaticVarLoader(varsFile);

            var loader = new FbuDataLoader(varLoader);

            var compressor = new Compressor();
            compressor.CompressionProgressChanged += CompProgressChanged;

            //var script = new Script(filepath, loader, varLoader, compressor);
            //script.Save(Path.Combine(outDir, "base.xml"));


            var script2 = new Script(Path.Combine(outDir, "base.xml"), varLoader);
            script2.SaveToNative(Path.Combine(outDir, "native"));

            //script2.Save(Path.Combine(outDir, "base2.xml"));
        }

        static void CompProgressChanged(object o, CompressionProgressEventArgs args)
        {
            Console.Write("\r Total compressed = {0}%", args.PercentComplete);
        }

    }
}
