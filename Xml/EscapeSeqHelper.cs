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
        public static string HiddenEntryTemplateFull = @"((%N|%O|%P|p)+)|((%FS)?-{40,}(%|\d|[A-Z])+)";

        /// <summary>
        /// Любая escape-последовательность в скрипте
        /// </summary>
        public static string AnyEscapeSec = @"%[A-Z]";

        /// <summary>
        /// Совпадние все строки этому шаблону детектирует Single-Translated запись
        /// </summary>
        public static string SingleLineTemplate = @".*⑳.*";

        /// <summary>
        /// Получает текст из японской строки (без escape-послед. в конце)
        /// </summary>
        public static string JapTextLineTemplate = @".*";

        /// <summary>
        /// Получает текст из японской строки (с escape-послед. в конце)
        /// </summary>
        public static string JapTextLineTemplateEscaped = @".+(?=((%K%P)|(%K%N)))";

        /// <summary>
        /// Получает текст из прямой речи (японская строка)
        /// </summary>
        public static string JapTextLineSpeak = @"(?<=「).+(?=」)";

        /// <summary>
        /// Получает текст из прямой речи (японская строка)
        /// </summary>
        public static string JapTextLineSpeak2 = @"(?<=『).+(?=』)";

        /// <summary>
        /// Получает текст из английского перевода
        /// </summary>
        public static string EnglishTextLineTemplate = @"(?<=//).*";

        /// <summary>
        /// Детектирует закоментированную строку в скрипте
        /// </summary>
        public static string CommentLineTemplate = @"^//.*";

        /// <summary>
        /// Детектирует русскую строчку
        /// </summary>
        public static string RusTextLineTemplate = @".*";

        /// <summary>
        /// Получает текст из Single-Translated записи
        /// </summary>
        public static string SingleTranslatedTextTemplate = "(?<=%LC).+(?=%FE)";
    }
}
