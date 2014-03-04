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

            int id = 1;
            var script = new List<Chapter>();

            Console.WriteLine("Loading Started..");
            foreach (var file in Directory.EnumerateFiles(filepath))
            {
                var chapter = loader.LoadFromNative(file, id);
                id += chapter.EntryCount;

                script.Add(chapter);
                Console.WriteLine(file);
                //chapter.SaveToFile(Path.ChangeExtension(file, "xml"));
            }

            Console.WriteLine("Compression Started..");

            var compressor = new Compressor();
            compressor.CompressionProgressChanged += CompProgresshanged;
            compressor.Compress(script);

            Console.WriteLine();
            
            foreach (var chapter in script)
            {
                chapter.SaveToFile(Path.Combine(outDir, Path.ChangeExtension(chapter.FileName, "xml")));
            }
            
        }

        static void CompProgresshanged(object o, CompressionProgressEventArgs args)
        {
            Console.Write("\r Total compressed = {0}%", args.PercentComplete);
        }
    }
}
