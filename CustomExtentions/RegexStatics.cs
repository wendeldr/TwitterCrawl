using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CustomExtentions
{
    public static class RegexStatics
    {
        internal static Regex SimpleTokenize = new Regex("([A-Z,a-z])\\w+");

        internal static Regex Whitespace = new Regex("[\\s\\p{Zs}]+");

        #region Punctuation
        private static string punctChars = "['\"“”‘’.?!…,:;]";
        #endregion

        #region URL
        private static string urlStart1 = "(?:https?://|\\bwww\\.)";
        //private static string commonTLDs = "(?:com|org|edu|gov|net|mil|aero|asia|biz|cat|coop|info|int|jobs|mobi|museum|name|pro|tel|travel|xxx)";
        //private static string ccTLDs = "(?:ac|ad|ae|af|ag|ai|al|am|an|ao|aq|ar|as|at|au|aw|ax|az|ba|bb|bd|be|bf|bg|bh|bi|bj|bm|bn|bo|br|bs|bt|" + "bv|bw|by|bz|ca|cc|cd|cf|cg|ch|ci|ck|cl|cm|cn|co|cr|cs|cu|cv|cx|cy|cz|dd|de|dj|dk|dm|do|dz|ec|ee|eg|eh|" + "er|es|et|eu|fi|fj|fk|fm|fo|fr|ga|gb|gd|ge|gf|gg|gh|gi|gl|gm|gn|gp|gq|gr|gs|gt|gu|gw|gy|hk|hm|hn|hr|ht|" + "hu|id|ie|il|im|in|io|iq|ir|is|it|je|jm|jo|jp|ke|kg|kh|ki|km|kn|kp|kr|kw|ky|kz|la|lb|lc|li|lk|lr|ls|lt|" + "lu|lv|ly|ma|mc|md|me|mg|mh|mk|ml|mm|mn|mo|mp|mq|mr|ms|mt|mu|mv|mw|mx|my|mz|na|nc|ne|nf|ng|ni|nl|no|np|" + "nr|nu|nz|om|pa|pe|pf|pg|ph|pk|pl|pm|pn|pr|ps|pt|pw|py|qa|re|ro|rs|ru|rw|sa|sb|sc|sd|se|sg|sh|si|sj|sk|" + "sl|sm|sn|so|sr|ss|st|su|sv|sy|sz|tc|td|tf|tg|th|tj|tk|tl|tm|tn|to|tp|tr|tt|tv|tw|tz|ua|ug|uk|us|uy|uz|" + "va|vc|ve|vg|vi|vn|vu|wf|ws|ye|yt|za|zm|zw)"; //TODO: remove obscure country domains?

        private static string commonTLDs = "(?:com)";
        private static string ccTLDs = "(?:ac)";
        private static string urlStart2 = "\\b(?:[A-Za-z\\d-])+(?:\\.[A-Za-z0-9]+){0,3}\\." + "(?:" + commonTLDs + "|" + ccTLDs + ")" + "(?:\\." + ccTLDs + ")?(?=\\W|$)";
        private static string urlBody = "(?:[^\\.\\s<>][^\\s<>]*?)?";
        private static string entity = "&(?:amp|lt|gt|quot);";
        private static string urlExtraCrapBeforeEnd = "(?:" + punctChars + "|" + entity + ")+?";
        private static string urlEnd = "(?:\\.\\.+|[<>]|\\s|$)";
        //private static string URLString = "(?:" + urlStart1 + "|" + urlStart2 + ")" + urlBody + "(?=(?:" + urlExtraCrapBeforeEnd + ")?" + urlEnd + ")";
        private static string URLString = "(?:" + urlStart1 + "|" + urlStart2 + ")" + urlBody + "(?="+ urlEnd + ")";


        internal static Regex URL = new Regex(URLString);
        #endregion

        #region Email
        private static string Bound = @"(?:\W|^|$)";
        private static string emailString = @"(?<=" + Bound + ")[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,4}(?=" + Bound + ")";

        internal static Regex Email = new Regex(emailString);
        #endregion

        #region Time
        private static string timeString = "\\d+(?::\\d+){1,2}";

        internal static Regex Time = new Regex(timeString);
        #endregion

        #region Numeric
        private static string numberWithCommasString = "(?:(?<!\\d)\\d{1,3},)+?\\d{3}" + "(?=(?:[^,\\d]|$))";
        private static string numDecimalString = "\\p{Sc}?\\d+(?:\\.\\d+)+%?";

        internal static Regex NumberWithCommas = new Regex(numberWithCommasString);
        internal static Regex NumberDecimal = new Regex(numDecimalString);
        #endregion

        #region Apostrophe
        private static string thingsThatSplitWords = "[^\\s\\.,?\"]";
        private static string embeddedApostropheString = thingsThatSplitWords + "+['’′]" + thingsThatSplitWords + "*";

        internal static Regex EmbeddedApostrophe = new Regex(embeddedApostropheString);
        #endregion

        #region Twitter Specific
        private static string HashtagString = "#[a-zA-Z0-9_]+"; //optional: lookbehind for \b
        private static string AtMentionString = "[@＠][a-zA-Z0-9_]+";

        internal static Regex Hashtag = new Regex(HashtagString); //optional: lookbehind for \b
        internal static Regex AtMention = new Regex(AtMentionString);
        #endregion

        #region Protected
        internal static Regex Protected = new Regex(OR(URLString, emailString, timeString, numberWithCommasString, numDecimalString,
            embeddedApostropheString, HashtagString, AtMentionString), RegexOptions.IgnoreCase);
        #endregion

        private static string OR(params string[] parts)
        {
            string prefix = "(?:";
            StringBuilder sb = new StringBuilder();
            foreach (string s in parts)
            {
                sb.Append(prefix);
                prefix = "|";
                sb.Append(s);
            }
            sb.Append(")");
            return sb.ToString();
        }
    }
}
