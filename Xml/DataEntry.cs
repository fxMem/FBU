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

#if INTERN
            string.Intern(_template);
#endif
        }

        public DataEntry(XElement xml, EntryType type)
        {
            _id = int.Parse(xml.Attribute(XmlDataValues.IdAttr).Value);
            _template = xml.Element(XmlDataValues.TemplateTitle).Value;
            _type = type;
        }

        public int Id { get { return _id; } }

        public EntryType Type { get { return _type; } }

        public string Template { get { return _template; } }

        public abstract TextElementBase this[string hash]
        {
            get;
        }

        public abstract IEnumerable<TextElementBase> EnumerateLines();

        /// <summary>
        /// Сравнивает текущую запись с предоставленной,
        /// и если есть совпадения, проставляет ссылки.
        /// </summary>
        /// <param name="simularEntry"></param>
        public virtual void TryLinkOut(DataEntry simularEntry)
        {

        }

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

        public abstract void DeleteTextLine(string hash);

        public abstract void ReplaceLineWithLink(string hash, TextElementBase linkedLine);

        public virtual void UpdateLinks(Script script)
        {

        }
    }

    public class HiddenEntry : DataEntry
    {
        public HiddenEntry(EntryType type, int id, string template)
            :base(type, id, template)
        {

        }

        public HiddenEntry(XElement xml)
            : base(xml, EntryType.Hidden)
        {
            if (Type != EntryType.Hidden)
            {
                throw new ArgumentOutOfRangeException("Can't init hidden entry with non-hidden xml template");
            }
        }

        public override string ToString(Language lang, IEnumerable<TemplateVariable> vars)
        {
            return Template;
        }

        public override TextElementBase this[string hash]
        {
            get { throw new NotSupportedException(@"Cant get line from a Hidden-Entry type."); }
        }

        public override IEnumerable<TextElementBase> EnumerateLines()
        {
            // ?
            return null;
        }

        public override void AddTextLine(TextElementBase text)
        {
            throw new NotSupportedException(@"Cant add line to a Hidden-Entry type.");
        }

        public override void DeleteTextLine(string hash)
        {
            throw new NotSupportedException("Cant Delete line from Hidden Entry");
        }

        public override void ReplaceLineWithLink(string hash, TextElementBase linkedLine)
        {
            throw new NotSupportedException("Cant replace line from SingleTranslated Entry");
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
            //var textLine = Regex.Match(template, EscapeSeqHelper.SingleTranslatedTextTemplate);
        }

        public SingleTextEntry(XElement xml)
            : base(xml, EntryType.SingleTranslated)
        {
            if (Type != EntryType.SingleTranslated)
            {
                throw new ArgumentOutOfRangeException("Can't init single-translated entry with non-s-t xml template");
            }

            // Получаем 1-й lang узел. Для данного типа записей он должен быть единственным
            var temp = xml.Element(XmlDataValues.LanguageTitle).Elements().FirstOrDefault();
            if (temp == null)
            {
                throw new ArgumentOutOfRangeException("Single-Translated xml entry must contain one lang node");
            }

            if (string.Equals(temp.Name.ToString(), XmlDataValues.TextLineTitle, StringComparison.Ordinal))
            {
                _text = new TextLine(temp, this, Language.NotSpecified);
            }
            else if (string.Equals(temp.Name.ToString(), XmlDataValues.TextLinkTitle, StringComparison.Ordinal))
            {
                _text = new TextLink(temp, this, Language.NotSpecified);
            }
        }

        public override string ToString(Language lang, IEnumerable<TemplateVariable> vars)
        {
            // TODO
            return _text.Value;
        }

        public override TextElementBase this[string hash]
        {
            get { return _text; }
        }

        public override IEnumerable<TextElementBase> EnumerateLines()
        {
            yield return _text;
        }

        public override void UpdateLinks(Script script)
        {
            _text.UpdateLinks(script);
        }

        public override void TryLinkOut(DataEntry simularEntry)
        {
            foreach (var line in simularEntry.EnumerateLines())
            {
                if (line.Language != _text.Language)
                {
                    continue;
                }
#if INTERN
            if (!object.ReferenceEquals(line.Value, _text.Value))
                    continue;
#else
            if (!string.Equals(line.Value, _text.Value, StringComparison.Ordinal))
                continue;
#endif

                ReplaceLineWithLink("", line);
            }
        }

        public override void AddTextLine(TextElementBase text)
        {
            if (text == null)
                throw new ArgumentNullException();

            _text = text;
        }

        public override void DeleteTextLine(string hash)
        {
            throw new InvalidOperationException("Cant Delete line from SingleTranslated Entry");
        }

        public override void ReplaceLineWithLink(string hash, TextElementBase linkedLine)
        {
            if (_text.Backlinks.Any())
            {
                throw new InvalidOperationException("Can't replace line because it has backlinks. Remove backlinks first.");
            }

            _text.Dispose();
            _text = new TextLink(this, 
                hash, linkedLine.Language, linkedLine.Contributor, linkedLine.DateTime, linkedLine);

            linkedLine.AddBacklink(_text);
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

        public DefaultEntry(XElement xml)
            : base(xml, EntryType.Default)
        {
            if (Type != EntryType.Default)
            {
                throw new ArgumentOutOfRangeException("Can't init default entry with non-default entry xml template");
            }

            _lines = new List<TextElementBase>();

            // Проходим по узлам Lang
            foreach (var node in xml.Elements(XmlDataValues.LanguageTitle))
            {
                Language lang = (Language)Enum.Parse(typeof(Language), node.Attribute(XmlDataValues.LanguageAttr).Value);

                // Проходим по каждой записи в языковом узле
                foreach (var line in node.Elements())
                {
                    // Определяем ее тип и добавляем объект соотв. типа в коллекцию 
                    if (string.Equals(line.Name.ToString(), XmlDataValues.TextLineTitle, StringComparison.Ordinal))
                    {
                        var newLine = new TextLine(line, this, lang);
                        _lines.Add(newLine);
                    }
                    else if (string.Equals(line.Name.ToString(), XmlDataValues.TextLinkTitle, StringComparison.Ordinal))
                    {
                        var newLine = new TextLink(line, this, lang);
                        _lines.Add(newLine);
                    }
                }
            }
        }
        

        public override string ToString(Language lang, IEnumerable<TemplateVariable> vars)
        {
            var temp = _lines.GetPrefferedForLanguage(lang);
            var line = temp != null ? Template.FillTemplate(temp.Value, vars, lang) : null;

            return line;
        }

        public override IEnumerable<TextElementBase> EnumerateLines()
        {
            return _lines;
        }

        public override void UpdateLinks(Script script)
        {
            for (int i = 0; i < _lines.Count; i++)
            {
                _lines[i].UpdateLinks(script);
            }
        }

        public override void TryLinkOut(DataEntry simularEntry)
        {
            foreach (var line in simularEntry.EnumerateLines())
            {
                if (line is TextLink)
                {
                    continue;
                }
                // Используется list, т.к. при использовании IEnumerable
                // нельзя изменять коллекцию
                for (int lineNum = 0; lineNum < _lines.Count; lineNum++ )
                {
                    var sourceLine = _lines[lineNum];

                    if (line.Language != sourceLine.Language)
                    {
                        continue;
                    }
                    
#if INTERN
                    if (!object.ReferenceEquals(line.Value, sourceLine.Value))
                        continue;
#else
                    if (!string.Equals(line.Value, sourceLine.Value, StringComparison.Ordinal))
                        continue;
#endif

                    ReplaceLineWithLink(sourceLine.Hash, line);
                }
                    
            }
        }

        public override TextElementBase this[string hash]
        {
            get 
            {
                return
                _lines.
                Where(line => string.Equals(line.Hash, hash, StringComparison.Ordinal)).
                FirstOrDefault();
            }
        }

        public override void AddTextLine(TextElementBase text)
        {
            if (text == null)
                throw new ArgumentNullException();

            _lines.Add(text);
        }

        public override void ReplaceLineWithLink(string hash, TextElementBase linkedLine)
        {
            DeleteTextLine(hash);
            var newLink = new TextLink(this, 
                hash, linkedLine.Language, linkedLine.Contributor, linkedLine.DateTime, linkedLine);

            _lines.Add(newLink);
            linkedLine.AddBacklink(newLink);
        }

        public override void DeleteTextLine(string hash)
        {
            if (string.IsNullOrWhiteSpace(hash))
            {
                return;
            }

            // Гарантируется уникальность хэша в пределах записи
            foreach (var line in _lines)
            {
                if (string.Equals(line.Hash, hash, StringComparison.Ordinal))
                {
                    if (line.Backlinks.Any())
                    {
                        throw new InvalidOperationException("Can't remove line because it has backlinks. Remove backlinks first.");
                    }

                    line.Dispose();
                    _lines.Remove(line);
                    break;
                }
            }
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

        /// <summary>
        /// Возвращает список узлов для указанного языка
        /// </summary>
        /// <param name="list"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public static IEnumerable<TextElementBase> GetLanguage(this IEnumerable<TextElementBase> list, Language lang)
        {
            if (list == null)
                return null;

            return list.Where(line => line.Language == lang);
        }

        /// <summary>
        /// Получает запись Preffered для данного языка
        /// </summary>
        /// <param name="list"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Возвращает первый по порядку текстовый элемент для языка
        /// </summary>
        /// <param name="list"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Определяет тип записи по певрой строке (при считывании)
        /// </summary>
        /// <param name="firstLine"></param>
        /// <returns></returns>
        public static EntryType CheckType(this string firstLine)
        {
            if (firstLine.IsMatchesToRegex(EscapeSeqHelper.CommentLineTemplate))
            {
                return EntryType.Comment;
            }
            else if (firstLine.IsMatchesToRegex(EscapeSeqHelper.HiddenEntryTemplateFull))
            {
                return EntryType.Hidden;
            }
            else if (firstLine.IsMatchesToRegex(EscapeSeqHelper.SingleLineTemplate))
            {
                return EntryType.SingleTranslated;
            }
            else
            {
                return EntryType.Default;
            }
        }

        
    }
}
