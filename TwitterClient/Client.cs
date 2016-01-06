using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Core.Interfaces.Streaminvi;

namespace Twitter
{
    public class Client
    {


        public TwitterClient()
        {
            _sampleStream = new Stream();
        }





        private static void TwitterStream()
        {
            _sampleStream = Stream.CreateSampleStream();
            stream.TweetReceived += (sender, args) =>
            {
                //Console.WriteLine(args.Tweet.Text);
            };
        }
    }
}
