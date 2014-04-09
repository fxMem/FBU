using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;

using Xml.Compression;

namespace Xml
{
    public class Script
    {
        private List<Chapter> _chapters;

        private ITemplateVarsLoader _varLoader;

        private ScriptVersion _version;

        public List<Chapter> Chapters { get { return _chapters; } }

        /// <summary>
        /// Загружает скрипт из мастер-файла
        /// </summary>
        /// <param name="masterFilePath"></param>
        public Script(XElement masterFileData)
        {
            

        }

        /// <summary>
        /// Загружает скрипт из файла нативного формата (из указанной директории)
        /// </summary>
        /// <param name="scriptDirectory"></param>
        public Script
            (string scriptDirectory,
            IFbuDataLoader loader,
            ITemplateVarsLoader varLoader,
            ICompressor compressor)
        {
            if (!Directory.Exists(scriptDirectory))
            {
                throw new DirectoryNotFoundException("You must specify directory with native script files");
            }

            _chapters = new List<Chapter>();
            int entryId = 1;
            foreach (var file in Directory.EnumerateFiles(scriptDirectory))
            {
                var chapter = loader.LoadFromNative(file, entryId);
                entryId += chapter.EntryCount;

                _chapters.Add(chapter);
            }

            compressor.Compress(_chapters);
            _version = new ScriptVersion(1, 0, 0);
        }

        /// <summary>
        /// Загрузить скрипт из мастер-файла (xml)
        /// </summary>
        /// <param name="filename"></param>
        public Script
            (string filename, 
            ITemplateVarsLoader varLoader
            )
        {
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException("Can't find file with script. + " + filename);
            }

            _varLoader = varLoader;

            _chapters = new List<Chapter>();
            XElement xml = XElement.Load(filename);

            var scriptVersion = xml.Attribute(XmlDataValues.ScriptVersionAttr).Value;
            _version = new ScriptVersion(scriptVersion);

            foreach (var chapter in xml.Elements(XmlDataValues.ChapterTitle))
            {
                var temp = new Chapter(chapter);
                _chapters.Add(temp);
            }

            for (int i = 0; i < _chapters.Count; i++)
            {
                _chapters[i].UpdateLinks(this);
            }
        }

        /// <summary>
        /// Обновить загруженный скрипт с помощью патч-файла
        /// </summary>
        /// <param name="patch"></param>
        public void Merge(XElement patch)
        {
            var _masterFIleRequiredVersion = patch.Attribute(XmlDataValues.ScriptVersionAttr).Value;
            var version = new ScriptVersion(_masterFIleRequiredVersion);

            // Патчфайл применяется только к определенной версии матер-файла
            if (_version.CompareTo(version) != 0)
            {
                throw new ArgumentOutOfRangeException(@"Cannot apply this patch file to current master file, 
                    destination master file version incorrect");
            }

            foreach (var entry in patch.Elements(XmlDataValues.EntryTitle))
            {
                var patchedEntryType = entry.GetEntryType();
                DataEntry patchedEntry = null;
                switch (patchedEntryType)
                {
                    case EntryType.Hidden :
                        {
                            throw new ArgumentOutOfRangeException("Patch file cannot contain hidden entries");
                        }
                    case EntryType.Default :
                        {
                            patchedEntry = new DefaultEntry(entry, EntryType.Default);
                            break;
                        }
                    case EntryType.SingleTranslated :
                        {
                            patchedEntry = new DefaultEntry(entry, EntryType.SingleTranslated);
                            break;
                        }
                }

                var sourceEntry = this.GetEntry(patchedEntry.Id);
                if (sourceEntry == null)
                {
                    throw new IndexOutOfRangeException(string.Format("Can't find in master-file entry with {0} id", patchedEntry.Id));
                }

                foreach (var line in patchedEntry.EnumerateLines())
                {
                    // Возможно, тут будет падать. Внимательно!
                    line.Approved = false;
                    sourceEntry.AddTextLine(line);
                }
            }
        }

        public DataEntry GetEntry(int id)
        {
            for(int chapterNum = 0; chapterNum < _chapters.Count; chapterNum++)
            {
                var chapter = _chapters[chapterNum];
                if (chapter.StartEntryId >  id)
                {
                    continue;
                }

                if (chapter.StartEntryId + chapter.EntryCount  - 1 < id)
                {
                    continue;
                }

                var temp = chapter.Elements.Where(entry => entry.Id == id);

                if (temp != null)
                {
                    return temp.FirstOrDefault();
                }
            }

            return null;
        }

        public void Save(string filename)
        {
            var temp = new XElement("script");
            temp.Add(new XAttribute(XmlDataValues.ScriptVersionAttr, _version.ToString()));

            foreach (var chapter in _chapters)
            {
                temp.Add(chapter.toXML());
            }

            temp.Save(filename);
        }

        public void SaveToNative(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var variables = _varLoader.GetVariables();

            foreach (var chapter in _chapters)
            {
                var filename = string.Format("{0}.txt", chapter.FileName);
                var filepath = Path.Combine(directory, filename);
                
                using (var writer = new StreamWriter(File.Create(filepath)))
                {
                    foreach (var entry in chapter.Elements)
                    {
                        writer.Write(entry.ToString(variables));

                        if (entry.Type == EntryType.Default)
                        {
                            writer.WriteLine();
                        }
                    }
                }

            }


        }

    }
}
