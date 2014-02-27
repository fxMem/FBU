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
        Chapter LoadFromNative(string filename, int startId);

        Chapter LoadFromXml(string filename);

        Chapter LoadFromXml(XElement xml);
    }
}
