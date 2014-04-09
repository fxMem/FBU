using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xml
{
    /// <summary>
    /// Класс описывает названия узлов, применяемые при построении xml-структуры
    /// Значения с постфиксом Title обозначают названия узлов,
    /// а с постфиксом Attr - названия атрибутов
    /// </summary>
    public static class XmlDataValues
    {
        /// <summary>
        /// Описывает корень всей стурктуры
        /// </summary>
        public static readonly string RootTitle = "dataroot";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string ChapterTitle = "chapter";

        /// <summary>
        /// Узел для переменных шаблона
        /// </summary>
        public static readonly string TVariablesTitle = "tplvars";

        /// <summary>
        /// Узел для переменной шаблона
        /// </summary>
        public static readonly string TVariableTitle = "tplvar";

        /// <summary>
        /// Название атрибута для указания имени переменной
        /// </summary>
        public static readonly string VarNameAttr = "varname";

        /// <summary>
        /// Узел для обозначения языка
        /// </summary>
        public static readonly string LanguageTitle = "lang";

        /// <summary>
        /// Название атрибута, используемого для спецификации конкретного языка в языковом узле
        /// </summary>
        public static readonly string LanguageAttr = "code";

        /// <summary>
        /// название узла, содержащего строку
        /// </summary>
        public static readonly string TextLineTitle = "textline";


        public static readonly string TextLinkTitle = "textlink";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string IdAttr = "id";
        /// <summary>
        /// 
        /// </summary>
        public static readonly string HashAttr = "hash";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string PrefferedAttr = "preffered";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string ApprovedAttr = "approved";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string ContributorAttr = "contributor";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string DateTimeAttr = "date-time";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string TargetIdAttr = "target-id";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string TargetHashAttr = "target-hash";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string BacklinkAttr = "backlink";

        /// <summary>
        /// Название узла для DataEntry
        /// </summary>
        public static readonly string EntryTitle = "entry";

        /// <summary>
        /// Атрибут типа Записи
        /// </summary>
        public static readonly string EntryTypeAttr = "type";

        /// <summary>
        /// Узел для шаблона записи
        /// </summary>
        public static readonly string TemplateTitle = "template";

        /// <summary>
        /// Аттрибут, указывающий файл для главы
        /// </summary>
        public static readonly string RootNameAttr = "name";

        /// <summary>
        /// Указывает версию скрипта
        /// </summary>
        public static readonly string ScriptVersionAttr = "version";



    }
}
