using CommentQuality.OouiForms.Stuff;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CommentQuality.OouiForms.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            var restApi = new RestApi();
            var commentCount = await restApi.GetCommentCount(entry.Text);
            editor.Text += $"\n{commentCount}";
        }
    }
}