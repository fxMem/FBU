﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xml
{
    public class ChapterProcessedEventArgs : EventArgs
    {
        public string ChapterName { get; set; }
    }
}
