using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xml
{
    public static class EscapeSeqHelper
    {
        /// <summary>
        /// Детектирует записть hidden-типа
        /// </summary>
        public static readonly string HiddenEntryTemplateFull = @"((%N|%O|%P|p)+)|((%FS)?-{40,}(%|\d|[A-Z])+)";

        /// <summary>
        /// Идентификатор для начальной escape-последовательности
        /// </summary>
        public static readonly string MetaStartGroupName = "metaStart";

        /// <summary>
        /// Идентификатор для конечной escape-последовательности
        /// </summary>
        public static readonly string MetaEndGroupName = "metaEnd";

        /// <summary>
        /// Идентификатор для текста
        /// </summary>
        public static readonly string TextGroupName = "text";

        /// <summary>
        /// Идентификатор для говорящего
        /// </summary>
        public static readonly string SpeakerGroupName = "speaker";

        /// <summary>
        /// Любая escape-последовательность в скрипте
        /// </summary>
        public static readonly string StartEscapeSecGroup = @"(?'" + MetaStartGroupName + @"'(%[A-Zp][A-Z\d]*)*)";
        public static readonly string EndEscapeSecGroup = @"(?'" + MetaEndGroupName + @"'(%[A-Zp][A-Z\d]*)*)";

        /// <summary>
        /// Блок текста
        /// </summary>
        public static readonly string AnyTextGroup = @"(?'" + TextGroupName + @"'.+?)";

        /// <summary>
        /// Говорящий. Подразумевается, что строка предварительно обработана в соответствии
        /// со списком метапеременных
        /// </summary>
        public static readonly string SpeakerGroup = @"(?'" + SpeakerGroupName + @"'{%.+%})";

        /// <summary>
        /// Общая строка-шаблон
        /// </summary>
        public static readonly string EscapedString =
            @"^" +
            StartEscapeSecGroup +
            SpeakerGroup + "?" +
            "「?" +
            AnyTextGroup  +
            "」?" +
            EndEscapeSecGroup + "$";


        public static readonly string TextLine =
            @"(?<=^" +
            StartEscapeSecGroup +
            SpeakerGroup + "?" +
            "「?)" +
            AnyTextGroup +
            "(?=」?" +
            EndEscapeSecGroup + "$)";

        /// <summary>
        /// Совпадние все строки этому шаблону детектирует Single-Translated запись
        /// </summary>
        public static readonly string SingleLineTemplate = @".*⑳.*";

        /// <summary>
        /// Получает текст из японской строки (без escape-послед. в конце)
        /// </summary>
        public static readonly string JapTextLineTemplate = @".*";

        /// <summary>
        /// Получает текст из японской строки (с escape-послед. в конце)
        /// </summary>
        public static readonly string JapTextLineTemplateEscaped = @".+(?=((%K%P)|(%K%N)))";

        /// <summary>
        /// Получает текст из прямой речи (японская строка)
        /// </summary>
        public static readonly string JapTextLineSpeak = @"(?<=「).+(?=」)";

        /// <summary>
        /// Получает текст из прямой речи (японская строка)
        /// </summary>
        public static readonly string JapTextLineSpeak2 = @"(?<=『).+(?=』)";

        /// <summary>
        /// Получает текст из английского перевода
        /// </summary>
        public static readonly string EnglishTextLineTemplate = @"(?<=//).*";

        /// <summary>
        /// Детектирует закоментированную строку в скрипте
        /// </summary>
        public static readonly string CommentLineTemplate = @"^//.*";

        /// <summary>
        /// Детектирует русскую строчку
        /// </summary>
        public static readonly string RusTextLineTemplate = @".*";

        /// <summary>
        /// Получает текст из Single-Translated записи
        /// </summary>
        public static readonly string SingleTranslatedTextTemplate = "(?<=%LC).+(?=%FE)";

        /// <summary>
        /// Извлекает название роута из имени файла
        /// </summary>
        public static readonly string RouteOfChapter = @"^[A-Z]+(?=\d*_)";

        /// <summary>
        /// Извлекает день из имени файла
        /// </summary>
        public static readonly string DayOfChapter = @"^(?<=[A-Z]+)\d+(?=_)";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string ValidNativeFileName = @"^[A-Z]+\d*_\d+[A-Z]*$";


    }
}
