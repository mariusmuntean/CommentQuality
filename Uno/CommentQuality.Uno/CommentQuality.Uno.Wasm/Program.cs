using System;
using Windows.UI.Xaml;

namespace CommentQuality.Uno.Wasm
{
    public class Program
    {
        private static App _app;

        private static void Main(string[] args)
        {
            Console.WriteLine("Go!");
            //Application.Start(_ => _app = new App());
            _app = new App();
        }
    }
}