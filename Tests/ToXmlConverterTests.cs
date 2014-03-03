using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text.RegularExpressions;

using Xml;

namespace Tests
{
    [TestClass]
    public class ToXmlConverterTests
    {
        [TestMethod]
        public void TestTemplateVariablesXml()
        {
            var filepath = @"C:\homeworks\fbu\script\";
            var varsFile = @"vars.xml";

            var str = @"悟「もうすぐ……おまえに……」%K%P";

            var r = Regex.Match(str, EscapeSeqHelper.TextLine);
            var es = r.Groups[EscapeSeqHelper.MetaStartGroupName];
            var ee = r.Groups[EscapeSeqHelper.MetaEndGroupName];
            var l = r.Groups[EscapeSeqHelper.TextGroupName];
            var s = r.Groups[EscapeSeqHelper.SpeakerGroupName];


            //foreach (var file in Directory.EnumerateFiles(filepath))
            //{
            //    var str = File.ReadAllLines(file);
                
            //}


        }
    }
}
