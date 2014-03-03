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

    public class Chapter
    {
        private List<DataEntry> _data;
        private string _filename;
        private RouteName _route;
        private int _day;

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

            var route = _filename.GetMatch(EscapeSeqHelper.RouteOfChapter);
            _route = (RouteName)Enum.Parse(typeof(RouteName), route.ToUpperInvariant());

            var day = filename.GetMatch(EscapeSeqHelper.DayOfChapter);
            if (!String.IsNullOrWhiteSpace(day))
            {
                _day = int.Parse(day);
            }
        }

        public string FileName { get { return _filename; } }

        public RouteName Route { get { return _route; } }

        public int Day { get { return _day; } }

        public int EntryCount { get { return _data.Count; } }

        public IEnumerable<DataEntry> Elements { get { return _data; } }

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

        public void SaveToFile(string filename)
        {
            var xml = toXML();

            xml.Save(filename);
        }

    }
}
