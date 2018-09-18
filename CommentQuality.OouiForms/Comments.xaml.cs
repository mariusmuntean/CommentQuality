using System;
using CommentQuality.Core.Stuff;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CommentQuality.OouiForms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Comments : ContentPage
    {
        private Editor _editor;
        private Entry _entry;

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var stack = new StackLayout();
            stack.Children.Add(new Label
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Text = "Enter a video URL"
            });
            _entry = new Entry
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.CenterAndExpand,
            };
            stack.Children.Add(_entry);
            var button = new Button
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Text = "Click me!"
            };
            stack.Children.Add(button);

            _editor = new Editor
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HeightRequest = 300
            };
            stack.Children.Add(_editor);

            stack.Spacing = 2;
            stack.VerticalOptions = LayoutOptions.StartAndExpand;
            this.Content = stack;

            // Add some logic to it
            var count = 0;
            button.Clicked += ButtonOnClicked;
        }

        private async void ButtonOnClicked(object sender, EventArgs e)
        {
            var restApi = new RestApi();
            var commentCount = await restApi.GetCommentCount(_entry.Text);
            _editor.Text += $"\n{commentCount}";
        }
    }
}