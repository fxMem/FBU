using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xml.Compression
{
    public class CompressionProgressEventArgs : EventArgs
    {
        public double PercentComplete { get; set; }
    }
}
