using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Xml
{
    public interface IFbuDataLoader
    {
        /// <summary>
        /// Загружает данные скрипта из нативного формата
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="startId"></param>
        /// <returns></returns>
        Chapter LoadFromNative(string filename, int startId);

        /// <summary>
        /// Загружает данные скрипта из xml-формата
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        Chapter LoadFromXml(string filename);

        Chapter LoadFromXml(XElement xml);
    }
}
