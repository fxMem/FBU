using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace Xml
{
    public class FbuDataLoader : IFbuDataLoader
    {
        private IEnumerable<TemplateVariable> _variables;

        public FbuDataLoader(
            ITemplateVarsLoader varLoader
            )
        {
            _variables = varLoader.GetVariables();

        }

        public Chapter LoadFromNative(string filename, int startId)
        {
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException(filename);
            }

            List<DataEntry> data;
            using (var reader = File.OpenText(filename))
            {
                data = ExtractElements(reader, startId);
            }

            var result = new Chapter(Path.GetFileNameWithoutExtension(filename), data);

            return result;
        }

        public Chapter LoadFromXml(string filename)
        {
            throw new NotImplementedException();
        }

        public Chapter LoadFromXml(System.Xml.Linq.XElement xml)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Извлекает все Записи из входного потока
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="startId"></param>
        /// <returns></returns>
        private List<DataEntry> ExtractElements(StreamReader reader, int startId)
        {
            var id = startId;
            var temp = new List<DataEntry>();

            while (!reader.EndOfStream)
            {
                var nextEntry = getNextEntry(reader, id);

                // Достигнут конец потока
                if (nextEntry == null)
                    break;

                temp.Add(nextEntry);
                id++;
            }

            return temp;
        }

        /// <summary>
        /// Считывает следующую запись из входного потока. 
        /// Если достигнут конец потока и записи не обнаружено, возвращается null
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="id">Id для считанной записи</param>
        /// <returns></returns>
        private DataEntry getNextEntry(StreamReader reader, int id)
        {
            DataEntry temp;

            while (!reader.EndOfStream)
            {
                var nextLine = reader.ReadLine();

                if (string.IsNullOrWhiteSpace(nextLine))
                    continue;

                var entryType = nextLine.CheckType();
                switch (entryType)
                {
                    case EntryType.Hidden:
                        {
                            temp = new HiddenEntry(EntryType.Hidden, id, nextLine);
                            return temp;
                        }
                    case EntryType.SingleTranslated:
                        {
                            temp = getSingleTranslatedEntry(nextLine, id);
                            return temp;
                        }
                    case EntryType.Default:
                        {
                            temp = getDefaultEntry(nextLine, reader, id);
                            return temp;
                        }
                    case EntryType.Comment :
                        {
                            // Пропускаем комментированные строки
                            continue;
                        }
                }
            }

            // Конец потока
            return null;
        }

        /// <summary>
        /// Считывает из входного потока Default-запись
        /// </summary>
        /// <param name="japLine"></param>
        /// <param name="reader"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private DataEntry getDefaultEntry(string japLine, StreamReader reader, int id)
        {
            // 1-я строка - на японском (оригинал)

            // Получам строку с переменными - заполнителями
            var lineWithoutVars = japLine.GetTemplateFromVars(_variables, Language.Jap);

            /// Получаем строчку текста
            var japTextLine = lineWithoutVars.GetLine();

            // Получаем "шаблон"
            var template = lineWithoutVars.GetTemplateFromLine(japTextLine);

            var temp = new DefaultEntry(EntryType.Default, id, template);
            temp.AddTextLine(new TextLine(temp, "JAP1", Language.Jap, "FROM_NATIVE", DateTime.Now, japTextLine));

            // Английским переводом считается 1-я закоментированная строчка после японской.
            // Остальные закоментированные строчки игнорируются
            bool englishTranslateFound = false;

            while (true)
            {
                var nextLine = reader.ReadLine();
                if (string.IsNullOrEmpty(nextLine))
                {
                    // Больше в этой группе строчек нет
                    return temp;
                }

                if (nextLine.IsMatchesToRegex(EscapeSeqHelper.CommentLineTemplate))
                {
                    if (englishTranslateFound)
                        continue;

                    // Строка с английским переводом
                    englishTranslateFound = true;
                    var engTextLine = nextLine.GetTextLine(Language.Eng);
                    temp.AddTextLine(new TextLine(temp, "ENG1", Language.Eng, "FROM_NATIVE", DateTime.Now, engTextLine));
                }
                else
                {
                    // Найдена незакоментированная строка

                    if (!englishTranslateFound)
                    {
                        // Английского перевода до сих пор не было найдено
                        // throw new InvalidDataException("Can't find english translation");
                    }

                    // Это русский перевод
                    var rusTextLine = nextLine.GetTextLine(Language.Rus);
                    temp.AddTextLine(new TextLine(temp, "RUS1", Language.Rus, "FROM_NATIVE", DateTime.Now, rusTextLine));
                    break;
                }
            }
            return temp;
        }

        /// <summary>
        /// Формирует Single Translated запись
        /// </summary>
        /// <param name="line"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private DataEntry getSingleTranslatedEntry(string line, int id)
        {
            var textLine = line.GetMatch(EscapeSeqHelper.SingleTranslatedTextTemplate);
            //var textLine = Regex.Match(line, EscapeSeqHelper.SingleTranslatedTextTemplate).Value;
            var template = line.GetTemplateFromLine(textLine);
            var temp = new SingleTextEntry(EntryType.SingleTranslated, id, template);
            temp.AddTextLine(new TextLine(temp, "ST1", Language.NotSpecified, "FROM_NATIVE", DateTime.Now, textLine));
            return temp;
        }
    }
}
