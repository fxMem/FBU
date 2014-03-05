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

        public List<Chapter> Chapters { get { return _chapters; } }

        /// <summary>
        /// Загружает скрипт из мастер-файла
        /// </summary>
        /// <param name="masterFilePath"></param>
        public Script(XElement masterFileData)
        {
            

        }

        /// <summary>
        /// Загружает скрипт из фалоы нативного формата (из указанной директории)
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

                if (chapter.StartEntryId + chapter.EntryCount < id)
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


    }
}
