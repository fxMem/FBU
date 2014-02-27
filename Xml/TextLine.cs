using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Xml
{
    public abstract class TextElementBase
    {
        private string _hash;
        private Language _lang;
        private bool _approved;
        private bool _preffered;
        private string _contributor;
        private DateTime _dateTime;
        private string _comment;
        private DataEntry _parent;
        private List<TextElementBase> _backlinks;

        public TextElementBase(DataEntry parent, string hash, Language lang, string contributor, DateTime time)
        {
            _parent = parent;
            _hash = hash;
            _lang = lang;
            _contributor = contributor;
            _dateTime = time;
            _backlinks = new List<TextElementBase>();
        }

        public Language Language { get { return _lang; } }

        public string Hash { get { return _hash; } }

        public string Contributor { get { return _contributor; } }

        public DateTime DateTime { get { return _dateTime; } }

        public bool Approved { get { return _approved; } set { _approved = value; } }

        public bool Preffered { get { return _preffered; } set { _preffered = value; } }

        public DataEntry ParentEntry { get { return _parent; } }

        public string Comment { get { return _comment; } set { _comment = value; } }

        public abstract string Value { get; set; }

        public IEnumerable<TextElementBase> Backlinks { get { return _backlinks; } }

        public void Addbacklink(TextElementBase link)
        {
            if (link == null)
                return;

            if (_backlinks.Contains(link))
                return;

            _backlinks.Add(link);
        }

        protected void valueUpdated()
        {
            _dateTime = DateTime.Now;
        }

        virtual public System.Xml.Linq.XElement toXML()
        {
            var xml = new XElement(XmlDataValues.TextLineTitle);

            if (Hash != null)
            {
                xml.Add(new XAttribute(XmlDataValues.HashAttr, Hash));
            }

            if (Preffered)
            {
                xml.Add(new XAttribute(XmlDataValues.PrefferedAttr, true));
            }

            if (!Approved)
            {
                xml.Add(new XAttribute(XmlDataValues.ApprovedAttr, false));
            }

            if (!String.IsNullOrWhiteSpace(Contributor))
            {
                xml.Add(new XAttribute(XmlDataValues.ContributorAttr, Contributor));
            }

            if (_backlinks.Any())
            {
                var attr = new StringBuilder();
                bool firstbackLink = true;
                foreach (var backlink in _backlinks)
                {
                    if (!firstbackLink)
                    {
                        attr.Append("&");
                        
                    }

                    attr.Append(backlink.ParentEntry.Id);
                    if (backlink.Hash != null)
                        attr.Append(":" + backlink.Hash);

                    if (firstbackLink)
                    {
                        firstbackLink = false;
                    }
                }

                xml.Add(new XAttribute(XmlDataValues.BacklinkAttr, attr.ToString()));
            }
            
            xml.Add(new XAttribute(XmlDataValues.DateTimeAttr, DateTime));

            return xml;
        }
    }

    public class TextLine : TextElementBase
    {
        private string _value;

        public TextLine(DataEntry parent, string hash, Language lang, string contributor, DateTime time)
            : base(parent, hash, lang, contributor, time)
        { }

        public TextLine(DataEntry parent, string hash, Language lang, string contributor, DateTime time, string value)
            :base(parent, hash, lang, contributor, time)
        {
            _value = value;
        }

        public override string Value 
        { 
            get { return _value; } 
            set 
            { 
                _value = value; 
                valueUpdated(); 
            } 
        }

        public override XElement toXML()
        {
            var xml = base.toXML();

            xml.Value = Value;

            return xml;
        }
    }

    public class TextLink : TextElementBase
    {
        private TextElementBase _linkedText;

        public TextLink(DataEntry parent, string hash, Language lang, string contributor, DateTime time, TextElementBase linkTo)
            :base(parent, hash, lang, contributor, time)
        {
            _linkedText = linkTo;
        }

        public override string Value
        {
            get { return _linkedText.Value; }
            set
            {
                throw new NotSupportedException(
                    @"Cannot change text value because this value refers to another TextLine. 
                    Try change original value instead");
            }
        }

        public override XElement toXML()
        {
            var xml = base.toXML();

            xml.Add(new XAttribute(XmlDataValues.TargetIdAttr, _linkedText.ParentEntry.Id));

            xml.Add(new XAttribute(XmlDataValues.TargetHashAttr, _linkedText.Hash));
            
            return xml;
        }

    }
}
