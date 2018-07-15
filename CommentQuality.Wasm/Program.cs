using System;
using CommentQuality.OouiForms;
using Ooui;
using Xamarin.Forms;

namespace CommentQuality.Wasm
{
    class Program
    {
        static void Main(string[] args)
        {
            Forms.Init();

            UI.Port = 8901;
            UI.Publish("/", new Comments().GetOouiElement());
        }
    }
}