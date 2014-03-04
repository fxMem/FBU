using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace Xml
{
    public static class TemplateHelper
    {
        public static string TemplateForVarReplace = @"(?<=^" +
            EscapeSeqHelper.StartEscapeSecGroup + "){0}(?=「)";

        public static string LineTagWithDelims = "{%LINE%}";

        public static string LineTag = "LINE";

        public static string TemplateDelim = "%";

       
        /// <summary>
        /// Заполняет все заменители актуальными значениями
        /// </summary>
        /// <param name="template"></param>
        /// <param name="textLine">Значение строки текста</param>
        /// <param name="vars">Перменные шаблона</param>
        /// <param name="lang">Какой язык быдет использоваться для замены переменными шаблона</param>
        /// <returns></returns>
        public static string FillTemplate(
            this string template, 
            string textLine, 
            IEnumerable<TemplateVariable> vars, 
            Language lang)
        {
            var regex = new Regex(template);
            var temp = regex.Replace(LineTag, textLine);
            foreach (var tVar in vars)
            {
                temp = regex.Replace(tVar.Name, tVar.Translates[lang]);
            }
            return temp;
        }

        /// <summary>
        /// Находит значения переменных шаблона в начале строки и заменяет их заполнителями
        /// </summary>
        /// <param name="line"></param>
        /// <param name="vars"></param>
        /// <param name="lang">На каком языке будут искаться значения</param>
        /// <returns></returns>
        public static string GetTemplateFromVars(
           this string line,
           IEnumerable<TemplateVariable> vars,
           Language lang)
        {
            string temp = line;
            foreach (var tVar in vars)
            {
                temp = Regex.Replace(
                    temp, 
                    string.Format(TemplateForVarReplace, tVar.Translates[lang]),
                    //tVar.Translates[lang],//@".*" + tVar.Translates[lang] + ".*", 
                    "{" + TemplateDelim + tVar.Name + TemplateDelim + "}");
            }
            return temp;
        }

        /// <summary>
        /// Заменяет подстроку в исходной строке LINE-тегом
        /// </summary>
        /// <param name="line">Исходная строка</param>
        /// <param name="textLine">Заменяемая подстрока</param>
        /// <returns></returns>
        public static string GetTemplateFromLine(
           this string line,
           string textLine
           )
        {
            string temp = line;
            
                temp = Regex.Replace(
                    line,
                    textLine,//@".*" + tVar.Translates[lang] + ".*",
                    LineTagWithDelims);
            
            return temp;
        }

        /// <summary>
        /// Получает строку тескста из raw строки
        /// </summary>
        /// <param name="line">Исходная строка. Значения переменных, если таковые в ней вствечаются, должны быть 
        /// предварительно заменены заполнителями</param>
        /// <param name="lang">Язык строки</param>
        /// <returns></returns>
        public static string GetTextLine(
          this string line,
            Language lang
          )
        {
            string template;

            switch (lang)
            {
                case Language.Jap :
                    {
                        return line.GetLine();
                    }
                case Language.Eng :
                    {
                        template = EscapeSeqHelper.EnglishTextLineTemplate;
                        break;
                    }
                case Language.Rus :
                    {
                        template = EscapeSeqHelper.RusTextLineTemplate;
                        break;
                    }
                default :
                    {
                        template = EscapeSeqHelper.RusTextLineTemplate;
                        break;
                    }
            }

            var temp = Regex.Match(line, template);

            if (!temp.Success)
            {
                throw new InvalidDataException("Cant parse string [" + temp + " ]");
            }

            return temp.Value;
        }

        /// <summary>
        /// Заменяет смысловую строку заполнителем для строки.
        /// </summary>
        /// <param name="source">Строка, в которой предварительно обработы все переменные шаблона</param>
        /// <returns></returns>
        public static string GetLine(
            this string source
            )
        {
            string temp = source.Trim();
            return Regex.Match(temp, EscapeSeqHelper.EscapedString).Groups[EscapeSeqHelper.TextGroupName].Value;
        }

        /// <summary>
        /// Формирует xml-описание для переменных шаблона
        /// </summary>
        /// <param name="vars"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public static XElement GetXmlForLanguage(this IEnumerable<TemplateVariable> vars, Language lang)
        {
            var result = new XElement(XmlDataValues.LanguageTitle);
            result.Add(new XAttribute(XmlDataValues.LanguageAttr, lang.ToString()));

            try
            {
                foreach (var variable in vars)
                {
                    var temp = new XElement(XmlDataValues.TVariableTitle, variable.Translates[lang]);
                    temp.Add(new XAttribute(XmlDataValues.VarNameAttr, variable.Name));

                    result.Add(temp);
                }
            }
            catch (KeyNotFoundException e)
            {
                // Некоторые переменные шаблона не содержат перевода на запрашиваемый язык
            }

            return result;
        }

        public static string GetMatch(this string sourceString, string template)
        {
            var result = Regex.Match(sourceString, template);

            if (!result.Success)
            {
                return null;
            }

            return result.Value;
        }

        /// <summary>
        /// Соответствует ли вся строка указанному шаблону
        /// </summary>
        /// <param name="text"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        public static bool IsMatchesToRegex(this string text, string template)
        {
            return (Regex.Match(text, template).Length == text.Length);
        }
    }
}
