using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace Xml
{

    public class Link
    {
        private int _targetId;

        private string _targetHash;

        private TextElementBase _target;

        public int TargetId { get { return _targetId; } }

        public string TargetHash { get { return _targetHash; } }

        public TextElementBase Target { get { return _target; } }

        public Link(int targetId, string targetHash)
        {
            _targetId = targetId;
            _targetHash = targetHash;
        }

        public Link(int targetId, string targetHash, TextElementBase target)
            :this(targetId, targetHash)
        {
            _target = target;
        }

        /// <summary>
        /// Обновляет ссылку. 
        /// </summary>
        /// <param name="script">Весь скрипт (из мастер-файла)</param>
        /// <param name="lang">Данное значение необходимо для разрешения "висячих" ссылок. Такие ссылки 
        /// указывают на строку с несуществующим хэшем. В таком случае ссылка проставляется на
        /// Preffered-узел для данного языка.</param>
        public void UpdateTarget(Script script, Language lang)
        {
            var entry = script.GetEntry(_targetId);

            var line = entry[_targetHash];
            if (line != null)
            {
                _target = line;
            }
            else
            {
                _target = entry.EnumerateLines().GetPrefferedForLanguage(lang);

                //throw new NotImplementedException("Requested hash are not contained in entry specified.");
            }
            
        }
    }

    public abstract class TextElementBase : IDisposable
    {
        private string _hash;
        private Language _lang;
        private bool _approved;
        private bool _preffered;
        private string _contributor;
        private DateTime _dateTime;
        private string _comment;
        private DataEntry _parent;

        private List<Link> _backlinks;

        public TextElementBase(DataEntry parent, string hash, Language lang, string contributor, DateTime time)
        {
            _parent = parent;
            _hash = hash;
            _lang = lang;
            _contributor = contributor;
            _dateTime = time;
            _backlinks = new List<Link>();
        }

        public TextElementBase(XElement xml, DataEntry parent, Language lang)
        {
            _parent = parent;
            _hash = xml.Attribute(XmlDataValues.HashAttr).Value;
            _lang = lang;
            _contributor = xml.Attribute(XmlDataValues.ContributorAttr).Value;
            _dateTime = DateTime.Parse(xml.Attribute(XmlDataValues.DateTimeAttr).Value);
            _backlinks = new List<Link>();

            var backlinksAttr = xml.Attribute(XmlDataValues.BacklinkAttr);
            if (backlinksAttr == null)
            {
                return;
            }

            var backlinks = backlinksAttr.Value.Split('&');
            foreach (var backlink in backlinks)
            {
                int targetId = backlink.getIdForBacklink();
                string targetHash = backlink.getHashForBacklink();

                var newBackLink = new Link(targetId, targetHash);
                _backlinks.Add(newBackLink);
            }
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

        public abstract void Dispose();

        public IEnumerable<Link> Backlinks { get { return _backlinks; } }

        public void AddBacklink(TextElementBase link)
        {
            if (link == null)
                return;

            if (_backlinks.Where(backlink => backlink.Target == link).Any())
                return;

            var temp = new Link(link.ParentEntry.Id, link.Hash, link);

            _backlinks.Add(temp);
        }

        public void UnLink(TextElementBase link)
        {
            if (link == null)
                return;

            _backlinks.RemoveAll(backlink => backlink.Target == link);
        }

        public virtual void UpdateLinks(Script script)
        {
            for (int i = 0; i < _backlinks.Count; i++)
            {
                var backlink = _backlinks[i];

                backlink.UpdateTarget(script, _lang);
            }
        }

        protected void valueUpdated()
        {
            _dateTime = DateTime.Now;
        }

        virtual public System.Xml.Linq.XElement toXML()
        {
            string title;

            if (this is TextLine)
                title = XmlDataValues.TextLineTitle;
            else
            {
                title = XmlDataValues.TextLinkTitle;
            }
            var xml = new XElement(title);

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

                    attr.Append(backlink.TargetId);
                    if (backlink.TargetHash != null)
                        attr.Append(":" + backlink.TargetHash);

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
#if INTERN
            string.Intern(_value);
#endif
        }

        public TextLine(XElement xml, DataEntry parent, Language lang)
            :base(xml, parent, lang)
        {
            if (!string.Equals(xml.Name.ToString(), XmlDataValues.TextLineTitle, StringComparison.Ordinal))
            {
                throw new ArgumentOutOfRangeException("This xml entry is not a textline");
            }

            _value = xml.Value;
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

        public override void Dispose()
        {
            
        }
    }

    public class TextLink : TextElementBase
    {
        private Link _linkedText;

        public TextLink(DataEntry parent, string hash, Language lang, string contributor, DateTime time, TextElementBase linkTo)
            :base(parent, hash, lang, contributor, time)
        {
            _linkedText = new Link(linkTo.ParentEntry.Id, linkTo.Hash, linkTo);
        }

        public TextLink(XElement xml, DataEntry parent, Language lang)
            :base(xml, parent, lang)
        {
            if (!string.Equals(xml.Name.ToString(), XmlDataValues.TextLinkTitle, StringComparison.Ordinal))
            {
                throw new ArgumentOutOfRangeException("This xml entry is not a textlink");
            }

            int targetId = int.Parse(xml.Attribute(XmlDataValues.TargetIdAttr).Value);
            string targetHash = xml.Attribute(XmlDataValues.TargetHashAttr).Value;

            _linkedText = new Link(targetId, targetHash);
        }

        public override void UpdateLinks(Script script)
        {
            base.UpdateLinks(script);
            _linkedText.UpdateTarget(script, Language);
        }

        public override string Value
        {
            get { return _linkedText.Target.Value; }
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

            xml.Add(new XAttribute(XmlDataValues.TargetIdAttr, _linkedText.TargetId));

            xml.Add(new XAttribute(XmlDataValues.TargetHashAttr, _linkedText.TargetHash));
            
            return xml;
        }

        public override void Dispose()
        {
            _linkedText.Target.UnLink(this);
            GC.SuppressFinalize(this);
        }

        ~TextLink()
        {
            Dispose();
        }

    }


}
