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
            _filename = filename;
            _data = elements;
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
