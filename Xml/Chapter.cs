using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Xml
{
    public enum RouteName
    {
        CO,
        COEP,
        SA,
        PR
    }

    /// <summary>
    /// Представляет собой скрипт из одного файла. 
    /// Внутри главы записи распологаются в порядке возрастания индекса.
    /// </summary>
    public class Chapter
    {
        private List<DataEntry> _data;
        private string _filename;
        private RouteName _route;
        private int _day;
        private int _startChapterId;

        public Chapter(string filename)
        {
            _filename = filename;
            _data = new List<DataEntry>();
        }

        public Chapter(string filename, List<DataEntry> elements)
        {
            // Предполагается, что скрипты грузятся только из файлов со стандартным именованием
            if (!filename.IsMatchesToRegex(EscapeSeqHelper.ValidNativeFileName))
            {
                throw new ArgumentOutOfRangeException("Filename must match native filename.");
            }

            _filename = filename;
            _data = elements;
            _startChapterId = _data[0].Id;

            var route = _filename.GetMatch(EscapeSeqHelper.RouteOfChapter);
            _route = (RouteName)Enum.Parse(typeof(RouteName), route.ToUpperInvariant());

            var day = filename.GetMatch(EscapeSeqHelper.DayOfChapter);
            if (!String.IsNullOrWhiteSpace(day))
            {
                _day = int.Parse(day);
            }
        }

        public Chapter(XElement xml)
        {
            if (!string.Equals(xml.Name.ToString(), XmlDataValues.ChapterTitle, StringComparison.Ordinal))
            {
                throw new ArgumentOutOfRangeException("Can't create chapter form non-chapter xml");
            }

            _filename = xml.Attribute(XmlDataValues.RootNameAttr).Value;

            bool firstEntry = true;
            _data = new List<DataEntry>();
            foreach (var entry in xml.Elements(XmlDataValues.EntryTitle))
            {
                if (firstEntry)
                {
                    // Запись должна всегда иметь id
                    var startId = entry.Attribute(XmlDataValues.IdAttr).Value;

                    _startChapterId = int.Parse(startId);
                    firstEntry = false;
                }

                // Определяем тип записи и создаем соотв. объект
                EntryType type;
                var typeAttr = entry.Attribute(XmlDataValues.EntryTypeAttr);
                if (typeAttr != null)
                {
                    type = (EntryType)Enum.Parse(typeof(EntryType), typeAttr.Value);
                }
                else
                {
                    type = EntryType.Default;
                }

                DataEntry temp;
                switch (type)
                {
                        
                    case EntryType.Hidden:
                        {
                            temp = new HiddenEntry(entry);
                            break;
                        }
                    case EntryType.SingleTranslated :
                        {
                            temp = new DefaultEntry(entry, EntryType.SingleTranslated);
                            break;
                        }
                    case EntryType.Default :
                        {
                            temp = new DefaultEntry(entry, EntryType.Default);
                            break;
                        }
                    default :
                        {
                            throw new ArgumentOutOfRangeException("Can't create entry of unknown type!");
                        }
                }
                _data.Add(temp);
            }

           
        }

        public string FileName { get { return _filename; } }

        public RouteName Route { get { return _route; } }

        public int Day { get { return _day; } }

        public int EntryCount { get { return _data.Count; } }

        public int StartEntryId { get { return _startChapterId; } }

        public IList<DataEntry> Elements { get { return _data; } }

        public XElement toXML()
        {
            var result = new XElement(XmlDataValues.ChapterTitle);
            result.Add(new XAttribute(XmlDataValues.RootNameAttr, FileName));

            foreach (var entry in _data)
            {
                result.Add(entry.toXML());
            }

            return result;
        }

        public void UpdateLinks(Script script)
        {
            for (int i = 0; i < _data.Count; i++)
            {
                _data[i].UpdateLinks(script);
            }

        }

        public void SaveToFile(string filename)
        {
            var xml = toXML();

            xml.Save(filename);
        }

        public void SaveToNativeFile(string filename)
        {


        }

    }
}
