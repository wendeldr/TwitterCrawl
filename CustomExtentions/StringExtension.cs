using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CustomExtentions
{
    public static class StringExtension
    {
        public static string SqueezeWhitespace(this string input)
        {
            return RegexStatics.Whitespace.Replace(input, " ");
        }

        public static MatchCollection URLTokenize(this string input)
        {
            return RegexStatics.URL.Matches(input);
        }

        public static MatchCollection SimpleTokenize(this string input)
        {
            return RegexStatics.SimpleTokenize.Matches(input);
        }
    }
}
