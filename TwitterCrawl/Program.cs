using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tweetinvi;
using Tweetinvi.Core.Credentials;

namespace TwitterCrawl
{
    public class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole(); // Create console window

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow(); // Get console window handle

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        public static Application WinApp { get; private set; }
        public static Window MainWindow { get; private set; }

        /// <summary>
        /// If the project is in window mode, GetConsoleWindow(); returns null, so here we test if the handle is null (zero in this case), 
        /// and if so, a console window needs to be created (only once). After this, GetConsoleWindow(); will always return a handle to use.
        /// </summary>
        static void ShowConsole()
        {
            var handle = GetConsoleWindow();
            if (handle == IntPtr.Zero)
                AllocConsole();
            else
                ShowWindow(handle, SW_SHOW);
        }

        static void HideConsole()
        {
            var handle = GetConsoleWindow();
            if (handle != null)
                ShowWindow(handle, SW_HIDE);
        }

        static void InitializeWindows()
        {
            WinApp = new Application();
            WinApp.Run(MainWindow = new MainWindow()); // note: blocking call
        }

        static void SetupTwitter(string CONSUMER_KEY, string CONSUMER_SECRET, string ACCESS_TOKEN, string ACCESS_TOKEN_SECRET)
        {
            Auth.SetUserCredentials(CONSUMER_KEY, CONSUMER_SECRET, ACCESS_TOKEN, ACCESS_TOKEN_SECRET);
            Auth.ApplicationCredentials = new TwitterCredentials(CONSUMER_KEY, CONSUMER_SECRET, ACCESS_TOKEN, ACCESS_TOKEN_SECRET);
        }

        // Required, as WPF needs to run in a Single Threaded Apartment environment. https://msdn.microsoft.com/en-us/library/windows/desktop/ms680112(v=vs.85).aspx
        [STAThread]
        static void Main(string[] args)
        {
            string CONSUMER_KEY = "DKt8dcLMMJ2BG5gm3zPDnyaIn";

            string CONSUMER_SECRET = "rAixpHVVuGTFlsae4Z2ymo7ZF0i1SIJeJ4Q5Ti7ZzTlNHlSsKe";

            string ACCESS_TOKEN = "4453302675-ok2iWq2fiCwfwTJLVSiYzkUO67isn4Kifb6JXvA";

            string ACCESS_TOKEN_SECRET = "KPeASVLcGJ7hpnqNRMavONUQO5Nk05KPeQsVfB512RZgV";

            SetupTwitter(CONSUMER_KEY, CONSUMER_SECRET, ACCESS_TOKEN, ACCESS_TOKEN_SECRET);

            ShowConsole(); // Show the console window (for Win App projects)


            string s = @"I'm quite heavily emotionally invested in this baby penguin, and if anything happens to him, I will NEVER forgive Kate Winslet. 
                #SnowChick That was the best New Year's broadcast I've ever done? 
                You all ABSOLUTELY ROCK THE FUCKING HOUSE, See you all next time. < 3 <3 < 3, 
                www.google.com , 1.0, U.N.K.L.E., 12:53 sevadus@sevadus.tv, dougie_wendelken@yahoo.com ";

            //var temp = Tokenize.WhitespaceTokenize(s);
            //Tokenize.tokenize(s);
            //Console.WriteLine("Opening window...");
            InitializeWindows(); // opens the WPF window and waits here
            //Console.WriteLine("Exiting main...");
        }
    }
}