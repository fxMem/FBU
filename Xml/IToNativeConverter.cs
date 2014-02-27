using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Xml
{
    public interface IToNativeConverter
    {
        string[] GetFromXml(XElement xml);
    }
}
