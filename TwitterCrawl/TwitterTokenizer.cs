using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace TwitterCrawl
{
    /// <summary>
    /// Twokenize -- a tokenizer designed for Twitter text in English and some other European languages.
    /// This is the Java version. If you want the old Python version, see: http://github.com/brendano/tweetmotif
    /// 
    /// This tokenizer code has gone through a long history:
    /// 
    /// (1) Brendan O'Connor wrote original version in Python, http://github.com/brendano/tweetmotif
    ///        TweetMotif: Exploratory Search and Topic Summarization for Twitter.
    ///        Brendan O'Connor, Michel Krieger, and David Ahn.
    ///        ICWSM-2010 (demo track), http://brenocon.com/oconnor_krieger_ahn.icwsm2010.tweetmotif.pdf
    /// (2a) Kevin Gimpel and Daniel Mills modified it for POS tagging for the CMU ARK Twitter POS Tagger
    /// (2b) Jason Baldridge and David Snyder ported it to Scala
    /// (3) Brendan bugfixed the Scala port and merged with POS-specific changes
    ///     for the CMU ARK Twitter POS Tagger  
    /// (4) Tobi Owoputi ported it back to Java and added many improvements (2012-06)
    /// 
    /// Current home is http://github.com/brendano/ark-tweet-nlp and http://www.ark.cs.cmu.edu/TweetNLP
    /// 
    /// There have been at least 2 other Java ports, but they are not in the lineage for the code here.
    /// </summary>
    public static class Twokenize
    {
        internal static Regex Contractions = new Regex("(?i)(\\w+)(n['’′]t|['’′]ve|['’′]ll|['’′]d|['’′]re|['’′]s|['’′]m)$");
        internal static Regex Whitespace = new Regex("[\\s\\p{Zs}]+");

        internal static string punctChars = "['\"“”‘’.?!…,:;]";
        //static String punctSeq   = punctChars+"+";	//'anthem'. => ' anthem '.
        internal static string punctSeq = "['\"“”‘’]+|[.?!,…]+|[:;]+"; //'anthem'. => ' anthem ' .
        internal static string entity = "&(?:amp|lt|gt|quot);";
        //  URLs

        // BTO 2012-06: everyone thinks the daringfireball regex should be better, but they're wrong.
        // If you actually empirically test it the results are bad.
        // Please see https://github.com/brendano/ark-tweet-nlp/pull/9

        internal static string urlStart1 = "(?:https?://|\\bwww\\.)";
        internal static string commonTLDs = "(?:com|org|edu|gov|net|mil|aero|asia|biz|cat|coop|info|int|jobs|mobi|museum|name|pro|tel|travel|xxx)";
        internal static string ccTLDs = "(?:ac|ad|ae|af|ag|ai|al|am|an|ao|aq|ar|as|at|au|aw|ax|az|ba|bb|bd|be|bf|bg|bh|bi|bj|bm|bn|bo|br|bs|bt|" + "bv|bw|by|bz|ca|cc|cd|cf|cg|ch|ci|ck|cl|cm|cn|co|cr|cs|cu|cv|cx|cy|cz|dd|de|dj|dk|dm|do|dz|ec|ee|eg|eh|" + "er|es|et|eu|fi|fj|fk|fm|fo|fr|ga|gb|gd|ge|gf|gg|gh|gi|gl|gm|gn|gp|gq|gr|gs|gt|gu|gw|gy|hk|hm|hn|hr|ht|" + "hu|id|ie|il|im|in|io|iq|ir|is|it|je|jm|jo|jp|ke|kg|kh|ki|km|kn|kp|kr|kw|ky|kz|la|lb|lc|li|lk|lr|ls|lt|" + "lu|lv|ly|ma|mc|md|me|mg|mh|mk|ml|mm|mn|mo|mp|mq|mr|ms|mt|mu|mv|mw|mx|my|mz|na|nc|ne|nf|ng|ni|nl|no|np|" + "nr|nu|nz|om|pa|pe|pf|pg|ph|pk|pl|pm|pn|pr|ps|pt|pw|py|qa|re|ro|rs|ru|rw|sa|sb|sc|sd|se|sg|sh|si|sj|sk|" + "sl|sm|sn|so|sr|ss|st|su|sv|sy|sz|tc|td|tf|tg|th|tj|tk|tl|tm|tn|to|tp|tr|tt|tv|tw|tz|ua|ug|uk|us|uy|uz|" + "va|vc|ve|vg|vi|vn|vu|wf|ws|ye|yt|za|zm|zw)"; //TODO: remove obscure country domains?
        internal static string urlStart2 = "\\b(?:[A-Za-z\\d-])+(?:\\.[A-Za-z0-9]+){0,3}\\." + "(?:" + commonTLDs + "|" + ccTLDs + ")" + "(?:\\." + ccTLDs + ")?(?=\\W|$)";
        internal static string urlBody = "(?:[^\\.\\s<>][^\\s<>]*?)?";
        internal static string urlExtraCrapBeforeEnd = "(?:" + punctChars + "|" + entity + ")+?";
        internal static string urlEnd = "(?:\\.\\.+|[<>]|\\s|$)";
        public static string url = "(?:" + urlStart1 + "|" + urlStart2 + ")" + urlBody + "(?=(?:" + urlExtraCrapBeforeEnd + ")?" + urlEnd + ")";


        // Numeric
        internal static string timeLike = "\\d+(?::\\d+){1,2}";
        //static String numNum     = "\\d+\\.\\d+";
        internal static string numberWithCommas = "(?:(?<!\\d)\\d{1,3},)+?\\d{3}" + "(?=(?:[^,\\d]|$))";
        internal static string numComb = "\\p{Sc}?\\d+(?:\\.\\d+)+%?";

        // Abbreviations
        internal static string boundaryNotDot = "(?:$|\\s|[“\\u0022?!,:;]|" + entity + ")";
        internal static string aa1 = "(?:[A-Za-z]\\.){2,}(?=" + boundaryNotDot + ")";
        internal static string aa2 = "[^A-Za-z](?:[A-Za-z]\\.){1,}[A-Za-z](?=" + boundaryNotDot + ")";
        internal static string standardAbbreviations = "\\b(?:[Mm]r|[Mm]rs|[Mm]s|[Dd]r|[Ss]r|[Jj]r|[Rr]ep|[Ss]en|[Ss]t)\\.";
        internal static string arbitraryAbbrev = "(?:" + aa1 + "|" + aa2 + "|" + standardAbbreviations + ")";
        internal static string separators = "(?:--+|―|—|~|–|=)";
        internal static string decorations = "(?:[♫♪]+|[★☆]+|[♥❤♡]+|[\\u2639-\\u263b]+|[\\ue001-\\uebbb]+)";
        internal static string thingsThatSplitWords = "[^\\s\\.,?\"]";
        internal static string embeddedApostrophe = thingsThatSplitWords + "+['’′]" + thingsThatSplitWords + "*";

        public static string OR(params string[] parts)
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

        //  Emoticons
        internal static string normalEyes = "(?iu)[:=]"; // 8 and x are eyes but cause problems
        internal static string wink = "[;]";
        internal static string noseArea = "(?:|-|[^a-zA-Z0-9 ])"; // doesn't get :'-(
        internal static string happyMouths = "[D\\)\\]\\}]+";
        internal static string sadMouths = "[\\(\\[\\{]+";
        internal static string tongue = "[pPd3]+";
        internal static string otherMouths = "(?:[oO]+|[/\\\\]+|[vV]+|[Ss]+|[|]+)"; // remove forward slash if http://'s aren't cleaned

        // mouth repetition examples:
        // @aliciakeys Put it in a love song :-))
        // @hellocalyclops =))=))=)) Oh well

        internal static string bfLeft = "(♥|0|o|°|v|\\$|t|x|;|\\u0CA0|@|ʘ|•|・|◕|\\^|¬|\\*)";
        internal static string bfCenter = "(?:[\\.]|[_-]+)";
        internal static string bfRight = "\\x0002";
        internal static string s3 = "(?:--['\"])";
        internal static string s4 = "(?:<|&lt;|>|&gt;)[\\._-]+(?:<|&lt;|>|&gt;)";
        internal static string s5 = "(?:[.][_]+[.])";
        internal static string basicface = "(?:(?i)" + bfLeft + bfCenter + bfRight + ")|" + s3 + "|" + s4 + "|" + s5;

        internal static string eeLeft = "[＼\\\\ƪԄ\\(（<>;ヽ\\-=~\\*]+";
        internal static string eeRight = "[\\-=\\);'\\u0022<>ʃ）/／ノﾉ丿╯σっµ~\\*]+";
        internal static string eeSymbol = "[^A-Za-z0-9\\s\\(\\)\\*:=-]";
        internal static string eastEmote = eeLeft + "(?:" + basicface + "|" + eeSymbol + ")+" + eeRight;


        public static string emoticon = OR("(?:>|&gt;)?" + OR(normalEyes, wink) + OR(noseArea, "[Oo]") + OR(tongue + "(?=\\W|$|RT|rt|Rt)", otherMouths + "(?=\\W|$|RT|rt|Rt)", sadMouths, happyMouths), "(?<=(?: |^))" + OR(sadMouths, happyMouths, otherMouths) + noseArea + OR(normalEyes, wink) + "(?:<|&lt;)?", basicface); //eastEmote.replaceFirst("2", "1"),
        // Standard version  :) :( :] :D :P
        // reversed version (: D:  use positive lookbehind to remove "(word):"
        // because eyes on the right side is more ambiguous with the standard usage of : ;
        //inspired by http://en.wikipedia.org/wiki/User:Scapler/emoticons#East_Asian_style
        // iOS 'emoji' characters (some smileys, some symbols) [\ue001-\uebbb]  
        // TODO should try a big precompiled lexicon from Wikipedia, Dan Ramage told me (BTO) he does this

        internal static string Hearts = "(?:<+/?3+)+"; //the other hearts are in decorations

        internal static string Arrows = "(?:<*[-―—=]*>+|<+[-―—=]*>*)|\\p{InArrows}+";

        // BTO 2011-06: restored Hashtag, AtMention protection (dropped in original scala port) because it fixes
        // "hello (#hashtag)" ==> "hello (#hashtag )"  WRONG
        // "hello (#hashtag)" ==> "hello ( #hashtag )"  RIGHT
        // "hello (@person)" ==> "hello (@person )"  WRONG
        // "hello (@person)" ==> "hello ( @person )"  RIGHT
        // ... Some sort of weird interaction with edgepunct I guess, because edgepunct 
        // has poor content-symbol detection.

        // This also gets #1 #40 which probably aren't hashtags .. but good as tokens.
        // If you want good hashtag identification, use a different regex.
        internal static string Hashtag = "#[a-zA-Z0-9_]+"; //optional: lookbehind for \b
                                                           //optional: lookbehind for \b, max length 15
        internal static string AtMention = "[@＠][a-zA-Z0-9_]+";

        // I was worried this would conflict with at-mentions
        // but seems ok in sample of 5800: 7 changes all email fixes
        // http://www.regular-expressions.info/email.html
        internal static string Bound = @"(?:\W|^|$)";
        public static string Email = @"(?<=" + Bound + ")[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,4}(?=" + Bound + ")";

        // We will be tokenizing using these regexps as delimiters
        // Additionally, these things are "protected", meaning they shouldn't be further split themselves.
        internal static Regex Protected = new Regex(OR(Hearts, url, Email)); //, timeLike, numberWithCommas, numComb, emoticon, Arrows, entity, punctSeq, arbitraryAbbrev, separators, decorations, embeddedApostrophe, Hashtag, AtMention));
        //numNum,

        // Edge punctuation
        // Want: 'foo' => ' foo '
        // While also:   don't => don't
        // the first is considered "edge punctuation".
        // the second is word-internal punctuation -- don't want to mess with it.
        // BTO (2011-06): the edgepunct system seems to be the #1 source of problems these days.  
        // I remember it causing lots of trouble in the past as well.  Would be good to revisit or eliminate.

        // Note the 'smart quotes' (http://en.wikipedia.org/wiki/Smart_quotes)
        internal static string edgePunctChars = "'\"“”‘’«»{}\\(\\)\\[\\]\\*&"; //add \\p{So}? (symbols)
        internal static string edgePunct = "[" + edgePunctChars + "]";
        internal static string notEdgePunct = "[a-zA-Z0-9]"; // content characters
        internal static string offEdge = "(^|$|:|;|\\s|\\.|,)"; // colon here gets "(hello):" ==> "( hello ):"
        internal static Regex EdgePunctLeft = new Regex(offEdge + "(" + edgePunct + "+)(" + notEdgePunct + ")");
        internal static Regex EdgePunctRight = new Regex("(" + notEdgePunct + ")(" + edgePunct + "+)" + offEdge);

        public static string splitEdgePunct(string input)
        {
            input = EdgePunctLeft.Replace(input, "$1$2 $3");
            input = EdgePunctRight.Replace(input, "$1 $2$3");
            return input;
        }

        private class Pair<T1, T2>
        {
            public T1 first;
            public T2 second;
            public Pair(T1 x, T2 y)
            {
                first = x;
                second = y;
            }
        }

        // The main work of tokenizing a tweet.
        private static IList<string> simpleTokenize(string text)
        {

            // Do the no-brainers first
            string splitPunctText = splitEdgePunct(text);

            int textLength = splitPunctText.Length;

            // BTO: the logic here got quite convoluted via the Scala porting detour
            // It would be good to switch back to a nice simple procedural style like in the Python version
            // ... Scala is such a pain.  Never again.

            // Find the matches for subsequences that should be protected,
            // e.g. URLs, 1.0, U.N.K.L.E., 12:53
            MatchCollection mat = Protected.Matches(splitPunctText);
            string[] matches = Protected.Split(splitPunctText);
            //Storing as List[List[String]] to make zip easier later on 
            IList<IList<string>> bads = new List<IList<string>>(); //linked list?
            IList<Pair<int?, int?>> badSpans = new List<Pair<int?, int?>>();
            //while (matches.find())
            //{
            //    //The spans of the "bads" should not be split.
            //    if (matches.start() != matches.end())
            //    { //unnecessary?
            //        IList<string> bad = new List<string>(1);
            //        bad.Add(StringHelperClass.SubstringSpecial(splitPunctText, matches.start(), matches.end()));
            //        bads.Add(bad);
            //        badSpans.Add(new Pair<int?, int?>(matches.start(), matches.end()));
            //    }
            //}

            //// Create a list of indices to create the "goods", which can be
            //// split. We are taking "bad" spans like 
            ////     List((2,5), (8,10)) 
            //// to create 
            /////    List(0, 2, 5, 8, 10, 12)
            //// where, e.g., "12" here would be the textLength
            //// has an even length and no indices are the same
            //IList<int?> indices = new List<int?>(2 + 2 * badSpans.Count);
            //indices.Add(0);
            //foreach (Pair<int?, int?> p in badSpans)
            //{
            //    indices.Add(p.first);
            //    indices.Add(p.second);
            //}
            //indices.Add(textLength);

            //// Group the indices and map them to their respective portion of the string
            //IList<IList<string>> splitGoods = new List<IList<string>>(indices.Count / 2);
            //for (int i = 0; i < indices.Count; i += 2)
            //{
            //	string goodstr = StringHelperClass.SubstringSpecial(splitPunctText, indices[i], indices[i + 1]);
            //	IList<string> splitstr = Arrays.asList(goodstr.Trim().Split(" ", true));
            //	splitGoods.Add(splitstr);
            //}

            ////  Reinterpolate the 'good' and 'bad' Lists, ensuring that
            ////  additonal tokens from last good item get included
            IList<string> zippedStr = new List<string>();
            //int i;
            //for (i = 0; i < bads.Count; i++)
            //{
            //	zippedStr = addAllnonempty(zippedStr, splitGoods[i]);
            //	zippedStr = addAllnonempty(zippedStr, bads[i]);
            //}
            //zippedStr = addAllnonempty(zippedStr, splitGoods[i]);

            //// BTO: our POS tagger wants "ur" and "you're" to both be one token.
            //// Uncomment to get "you 're"
            ///*ArrayList<String> splitStr = new ArrayList<String>(zippedStr.size());
            //for(String tok:zippedStr)
            //	splitStr.addAll(splitToken(tok));
            //zippedStr=splitStr;*/

            return zippedStr;
        }

        private static IList<string> addAllnonempty(IList<string> master, IList<string> smaller)
        {
            foreach (string s in smaller)
            {
                string strim = s.Trim();
                if (strim.Length > 0)
                {
                    master.Add(strim);
                }
            }
            return master;
        }
        /// <summary>
        /// "foo   bar " => "foo bar" </summary>
        public static string squeezeWhitespace(string input)
        {
            return Whitespace.Replace(input, " ");
        }

        // Final pass tokenization based on special patterns
        //private static IList<string> splitToken(string token)
        //{

        //	//Matcher m = Contractions.matcher(token);
        //	//if (m.find())
        //	//{
        //	//	string[] contract = new string[] { m.group(1), m.group(2) };
        //	//	return Arrays.asList(contract);
        //	//}
        //	//string[] contract = new string[] { token };
        //	//return Arrays.asList(contract);
        //}

        /// <summary>
        /// Assume 'text' has no HTML escaping. * </summary>
        public static IList<string> tokenize(string text)
        {
            return simpleTokenize(squeezeWhitespace(text));
        }


        /// <summary>
        /// Twitter text comes HTML-escaped, so unescape it.
        /// We also first unescape &amp;'s, in case the text has been buggily double-escaped.
        /// </summary>
        public static string normalizeTextForTagger(string text)
        {
            //text = text.replaceAll("&amp;", "&");
            //text = StringEscapeUtils.unescapeHtml(text);
            return text;
        }

        /// <summary>
        /// This is intended for raw tweet text -- we do some HTML entity unescaping before running the tagger.
        /// 
        /// This function normalizes the input text BEFORE calling the tokenizer.
        /// So the tokens you get back may not exactly correspond to
        /// substrings of the original text.
        /// </summary>
        public static IList<string> tokenizeRawTweetText(string text)
        {
            IList<string> tokens = tokenize(normalizeTextForTagger(text));
            return tokens;
        }
    }
}