using Microsoft.VisualStudio.TestTools.UnitTesting;
using CustomExtentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CustomExtentions.Tests
{
    [TestClass()]
    public class StringExtensionTests
    {
        [TestMethod()]
        public void SqueezeWhitespaceTest()
        {
            string s = "   This is a     Test  String. ";
            Assert.IsTrue(s.SqueezeWhitespace() == " This is a Test String. ", s);
        }

        [TestClass()]
        public class URLTESTS
        {
            [TestMethod()]
            public void URLTokenize0()
            {
                string s = "Test urls: www.google.com, yahoo.com, https://www.facebook.com";

                MatchCollection m = s.URLTokenize();

                Assert.IsTrue(m[0].ToString() == "www.googe.com");
                Assert.IsTrue(m[2].ToString() == "yahoo.com");
                Assert.IsTrue(m[3].ToString() == "https://www.facebook.com");
            }

            [TestMethod()]
            public void URLTokenize1()
            {
                string s = "yahoo.com, google.com,";

                MatchCollection m = s.URLTokenize();

                Assert.IsTrue(m[0].ToString() == "yahoo.com");
                Assert.IsTrue(m[2].ToString() == "google.com");
            }
        }
    }
}