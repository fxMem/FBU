using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xml;

namespace Xml.Compression
{
    public class Compressor : ICompressor
    {
        public void Compress(IList<Chapter> data)
        {
            int entryCount = data.Sum(chapter => chapter.EntryCount);
            int threshhold = entryCount / 100;
            int counter = 0;

            for (int chapterNum = 0; chapterNum < data.Count; chapterNum++ )
            {
                var chapter = data[chapterNum];

                for (int entryNum = 0; entryNum < chapter.EntryCount; entryNum++)
                {
                    counter++;

                    var entry = chapter.Elements[entryNum];
                    data.LookForMatches(entry, chapterNum, entryNum + 1);

                    if (counter % threshhold == 0)
                    {
                        OnProgressChanged(new CompressionProgressEventArgs { PercentComplete = Math.Round((double)(100 * counter) / entryCount) });
                    }
                }

                //foreach (var line in entry.EnumerateLines())
                //{
                //    if (line is TextLink)
                //    {
                //        continue;
                //    }

                //    foreach (var match in line.EnumerateLineMatches(entry.EnumerateTemplateMatches(data)))
                //    {
                //        match.ParentEntry.ReplaceLineWithLink(line.Hash, line);
                //    }
                //}
                //Console.Write("\r{0} / {1}", counter, entryCount);
            }
        }


        public event EventHandler<CompressionProgressEventArgs> CompressionProgressChanged;

        protected void OnProgressChanged(CompressionProgressEventArgs args)
        {
            var temp = CompressionProgressChanged;

            if (temp != null)
            {
                temp(this, args);
            }
        }
    }

    static class CompressorHelper
    {
        private static IEnumerable<TextElementBase> _emptyMatch = new List<TextElementBase>();

        public static void LookForMatches(this IList<Chapter> data, DataEntry startEntry, int startChapterNum, int startEntryNum)
        {
            for (int chapterNum = startChapterNum; chapterNum < data.Count; chapterNum++)
            {
                var chapter = data[chapterNum];

                for (int entryNum = 0; entryNum < chapter.EntryCount; entryNum++)
                {
                    var entry = chapter.Elements[entryNum];

                    if (startEntry == entry)
                    {
                        continue;
                    }

                    if (entry.Type != startEntry.Type)
                    {
                        continue;
                    }
#if INTERN
                    if (!object.ReferenceEquals(entry.Template, startEntry.Template))
                        continue;
#else
                    if (!string.Equals(entry.Template, startEntry.Template, StringComparison.Ordinal))
                        continue;
#endif
                    try
                    {
                        entry.TryLinkOut(startEntry);
                    }
                    catch (InvalidOperationException)
                    {
                        // При попытке удалить элемент, на который ссылаются
                    }
                    
                }
            }
        }

        public static IEnumerable<DataEntry> EnumerateTemplateMatches(this DataEntry sourceEntry, IEnumerable<Chapter> data)
        {
            foreach (var chapter in data)
            {
                foreach (var entry in chapter.Elements)
                {
                    if ((string.Equals(entry.Template, sourceEntry.Template, StringComparison.Ordinal)) &&
                        (entry != sourceEntry)
                        )
                    {
                        yield return entry;
                    }
                }
            }
        }

        public static IEnumerable<DataEntry> EnumerateEntries(this IEnumerable<Chapter> data)
        {
            foreach (var chapter in data)
            {
                foreach (var entry in chapter.Elements)
                {
                    if (entry.Type == EntryType.Hidden)
                    {
                        // Не заменяем записи hidden - типа
                        continue;
                    }

                    yield return entry;
                }
            }
        }

        public static IEnumerable<TextElementBase> EnumerateLineMatches(this TextElementBase sourceLine, IEnumerable<DataEntry> simularEntries)
        {
            var language = sourceLine.Language;
            var sourceString = sourceLine.ToString();
            
            foreach (var entry in simularEntries)
            {
                foreach (var line in entry.EnumerateLines())
                {
                    if (line is TextLink)
                    {
                        continue;
                    }

                    if (line.Language != language)
                    {
                        continue;
                    }

                    if (!string.Equals(line.ToString(), sourceString, StringComparison.Ordinal))
                    {
                        continue;
                    }

                    yield return line;
                }
            }

        }



    }
}
