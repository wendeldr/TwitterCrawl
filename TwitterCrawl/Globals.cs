using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Core.Interfaces.Streaminvi;


namespace TwitterCrawl
{
    public class Globals
    {
        private static ISampleStream stream;

        public static ISampleStream Stream
        {
            get
            {
                return stream;
            }

            set
            {
                stream = value;
            }
        }

        /// <summary>
        ///  An enumeration of sorting options to be used.
        /// </summary>
        public enum SortOrder
        {
            Ascending, // from small to big numbers or alphabetically. 
            Descending // from big to small number or reversed alphabetical order 
        }



    }
}
