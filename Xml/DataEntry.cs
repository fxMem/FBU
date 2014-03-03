using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Xml
{
    public enum EntryType
    {
        Hidden,
        SingleTranslated,
        Default,
        Comment
    }

    public abstract class DataEntry
    {
        private int _id;
        private EntryType _type;
        private string _template;

        public DataEntry(EntryType type, int id, string template)
        {
            _id = id;
            _template = template;
            _type = type;
        }

        public int Id { get { return _id; } }

        public EntryType Type { get { return _type; } }

        public string Template { get { return _template; } }

        public virtual XElement toXML()
        {
            var xml = new XElement(XmlDataValues.EntryTitle);

            xml.Add(new XAttribute(XmlDataValues.IdAttr, Id));

            if (Type != EntryType.Default)
                xml.Add(new XAttribute(XmlDataValues.EntryTypeAttr, Type.ToString()));

            var template = new XElement(XmlDataValues.TemplateTitle, Template);
            xml.Add(template);

            return xml;
        }

        public abstract string ToString(Language lang, IEnumerable<TemplateVariable> vars);

        public abstract void AddTextLine(TextElementBase text);
    }

    public class HiddenEntry : DataEntry
    {
        public HiddenEntry(EntryType type, int id, string template)
            :base(type, id, template)
        {

        }

        public override string ToString(Language lang, IEnumerable<TemplateVariable> vars)
        {
            return Template;
        }

        public override void AddTextLine(TextElementBase text)
        {
            throw new NotSupportedException(@"Cant add line to a Hidden-Entry type.");
        }

        public override XElement toXML()
        {
            return base.toXML();
        }
    }

    public class SingleTextEntry : DataEntry
    {
        private TextElementBase _text;

        public SingleTextEntry(EntryType type, int id, string template)
            : base(type, id, template)
        {
            var textLine = Regex.Match(template, EscapeSeqHelper.SingleTranslatedTextTemplate);


        }

        public override string ToString(Language lang, IEnumerable<TemplateVariable> vars)
        {
            // TODO
            return Template;
        }

        public override void AddTextLine(TextElementBase text)
        {
            if (text == null)
                throw new ArgumentNullException();

            _text = text;
        }

        public override XElement toXML()
        {
            var xml = base.toXML();
            var langNode = new XElement(XmlDataValues.LanguageTitle);
            langNode.Add(_text.toXML());
            xml.Add(langNode);

            return xml;
        }
    }

    public class DefaultEntry : DataEntry
    {
        private List<TextElementBase> _lines;

        public DefaultEntry(EntryType type, int id, string template)
            :base(type, id, template)
        {
            _lines = new List<TextElementBase>();
        }

        public override string ToString(Language lang, IEnumerable<TemplateVariable> vars)
        {
            var temp = _lines.GetPrefferedForLanguage(lang);
            var line = temp != null ? Template.FillTemplate(temp.Value, vars, lang) : null;

            return line;
        }

        public override void AddTextLine(TextElementBase text)
        {
            if (text == null)
                throw new ArgumentNullException();

            _lines.Add(text);
        }

        public override XElement toXML()
        {
            var xml = base.toXML();

            xml.AddTextsForLanguage(_lines, Language.Jap);

            xml.AddTextsForLanguage(_lines, Language.Eng);

            xml.AddTextsForLanguage(_lines, Language.Rus);

            return xml;
        }
    }


    public static class DataEntryHelper
    {
        /// <summary>
        /// Добавляет в xml-узел Текстовые элементы определенного языка
        /// </summary>
        /// <param name="node"></param>
        /// <param name="list"></param>
        /// <param name="lang"></param>
        public static void AddTextsForLanguage(
            this XElement node, 
            IEnumerable<TextElementBase> list,
            Language lang)
        {
            var texts = list.GetLanguage(lang);

            if (!texts.Any())
                return;

            var langNode = new XElement(XmlDataValues.LanguageTitle);
            langNode.Add(new XAttribute(XmlDataValues.LanguageAttr, lang.ToString()));

            foreach (var text in texts)
            {
                langNode.Add(text.toXML());
            }

            node.Add(langNode);
        }

        public static IEnumerable<TextElementBase> GetLanguage(this IEnumerable<TextElementBase> list, Language lang)
        {
            if (list == null)
                return null;

            return list.Where(line => line.Language == lang);
        }

        public static TextElementBase GetPrefferedForLanguage(this IEnumerable<TextElementBase> list, Language lang)
        {
            if (list == null)
                return null;

            var lines = list.Where(line => (line.Language == lang) && (line.Preffered));
            if (lines.Any())
            {
                return lines.First();
            }
            else
            {
                return lines.GetFirstForLanguage(lang);
            }
        }

        public static TextElementBase GetFirstForLanguage(this IEnumerable<TextElementBase> list, Language lang)
        {
            if (list == null)
                return null;

            var lines = list.Where(line => line.Language == lang);
            if (lines.Any())
            {
                return lines.First();
            }

            return null;
        }

        public static EntryType CheckType(this string firstLine)
        {
            if (firstLine.IsMatchesToRegex(EscapeSeqHelper.HiddenEntryTemplateFull))
            {
                return EntryType.Hidden;
            }
            else if (firstLine.IsMatchesToRegex(EscapeSeqHelper.SingleLineTemplate))
            {
                return EntryType.SingleTranslated;
            }
            else if (firstLine.IsMatchesToRegex(EscapeSeqHelper.CommentLineTemplate))
            {
                return EntryType.Comment;
            }
            else
            {
                return EntryType.Default;
            }
        }

        
    }
}
