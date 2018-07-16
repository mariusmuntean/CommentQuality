using CommentQuality.OouiForms.Views;
using Ooui;
using Xamarin.Forms;

namespace CommentQuality.Wasm
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Forms.Init();

            UI.Port = 8901;
            UI.Publish("/", new HomePage().GetOouiElement());
        }
    }
}