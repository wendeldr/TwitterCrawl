using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tweetinvi;
using Tweetinvi.Core.Interfaces.Streaminvi;
using LanguageProcessing;
using CustomExtentions;
using System.Text.RegularExpressions;


namespace TwitterCrawl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            TwitterSampleStream();
        }

        private void TwitterSampleStream()
        {
            ListWordsByFrequency frequencyList = new ListWordsByFrequency();
            var logFile = System.IO.File.ReadAllLines(@"C:\Users\Daniel\Source\Repos\TwitterCrawl\positive-words.txt");
            List <string> StringList = new List<string>(logFile);
            Globals.Stream = Stream.CreateSampleStream();
            int incomming = 0;
            int success = 0;

            Globals.Stream.TweetReceived += (sender, args) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    incomming++;
                    label.Content = incomming;
                    label1.Content = success;

                    string tweet = args.Tweet.ToString();
                    bool test = StringList.Any(tweet.Contains);
            
                    if (test)
                    {
                        success++;
                        txtBxTwitterFeed.Clear();
                        txtBxTwitterFeed.Text = tweet;

                        var matchingvalues = StringList.FindAll(tweet.Contains);

                        StringBuilder builder = new StringBuilder();
                        foreach (string v in matchingvalues) // Loop through all strings
                        {
                            builder.Append(v).Append(" "); // Append string to StringBuilder
                        }

                        frequencyList.split(builder.ToString());
                        rTxtBxFrequency.Document.Blocks.Clear();
                        rTxtBxFrequency.Document.Blocks.Add(new Paragraph(new Run(frequencyList.output)));
                    }

                }));
            };
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            Globals.Stream.StartStreamAsync();
        }



        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            Globals.Stream.StopStream();
        }
    }
}
