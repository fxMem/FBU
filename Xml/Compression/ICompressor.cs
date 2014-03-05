using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xml.Compression
{
    /// <summary>
    /// Ищет соврпадающие строки в скрипте и заменяет их ссылками
    /// </summary>
    public interface ICompressor
    {
        void Compress(IList<Chapter> data);

        event EventHandler<CompressionProgressEventArgs> CompressionProgressChanged;

    }
}
