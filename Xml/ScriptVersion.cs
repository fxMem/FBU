using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Xml
{
    public class ScriptVersion : IComparable<ScriptVersion>
    {
        private string _inputTemplate = @"^(?'major'\d+)\.(?'minor'\d+)\.(?'sub'\d+)$";

        private string _inputTemplate2 = @"^{\d+}\.{\d+}\.{\d+}$";
        private string _test = @"\d+";

        private string _outputTemplate = "{0}.{1}.{2}";

        private Regex _match ;

        public int MajorVersion { get; private set; }

        public int MinorVersion { get; private set; }

        public int SubVersion { get; private set; }

        public ScriptVersion(string value)
        {
            _match = new Regex(_inputTemplate);

            var temp = _match.Match(value);

            var t = Regex.Matches(value, _test);

            if (!temp.Groups["major"].Success)
            {
                throw new ArgumentOutOfRangeException("Version value must be in following format: XX.XX.XX");
            }
            if (!temp.Groups["minor"].Success)
            {
                throw new ArgumentOutOfRangeException("Version value must be in following format: XX.XX.XX");
            }
            if (!temp.Groups["sub"].Success)
            {
                throw new ArgumentOutOfRangeException("Version value must be in following format: XX.XX.XX");
            }

            MajorVersion = int.Parse(temp.Groups["major"].Value);

            MinorVersion = int.Parse(temp.Groups["minor"].Value);

            SubVersion = int.Parse(temp.Groups["sub"].Value);
        }

        public ScriptVersion(int major, int minor, int sub)
        {
            MajorVersion = major;

            MinorVersion = minor;

            SubVersion = sub;
        }


        public static bool operator <(ScriptVersion first, ScriptVersion second)
        {
            if (first.CompareTo(second) < 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator >(ScriptVersion first, ScriptVersion second)
        {
            if (first.CompareTo(second) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int CompareTo(ScriptVersion other)
        {
            if (MajorVersion < other.MajorVersion)
            {
                return -1;
            }
            else if (MajorVersion > other.MajorVersion)
            {
                return 1;
            }

            if (MinorVersion < other.MinorVersion)
            {
                return -1;
            }
            else if (MinorVersion > other.MinorVersion)
            {
                return 1;
            }

            if (SubVersion < other.SubVersion)
            {
                return -1;
            }
            else if (SubVersion > other.SubVersion)
            {
                return 1;
            }

            return 0;
        }

        public override string ToString()
        {
            return string.Format(_outputTemplate, MajorVersion, MinorVersion, SubVersion);
        }
    }
}
