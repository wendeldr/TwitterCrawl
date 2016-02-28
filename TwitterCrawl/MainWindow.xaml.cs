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
            var logFile = System.IO.File.ReadAllLines("C:\\Users\\Daniel\\Desktop\\positive-words.txt");
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
                    bool test = StringList.Any(args.Tweet.ToString().Contains);

                    if (test)
                    {
                        success++;
                        txtBxTwitterFeed.Clear();
                        txtBxTwitterFeed.Text = args.Tweet.ToString();
                                                
                        //frequencyList.split(args.Tweet.ToString());
                        //rTxtBxFrequency.Document.Blocks.Clear();
                        //rTxtBxFrequency.Document.Blocks.Add(new Paragraph(new Run(frequencyList.output)));
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
