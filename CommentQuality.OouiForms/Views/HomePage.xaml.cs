using CommentQuality.OouiForms.Stuff;
using System;
using Google.Apis.YouTube.v3;
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
            var threadListResponse = await restApi.GetCommentThreads(entry.Text, "snippet,replies", null);

            for (var index = 0; index < threadListResponse.Items.Count; index++)
            {
                var commentThread = threadListResponse.Items[index];
                editor.Text += $"\n{index} {commentThread.Snippet?.TopLevelComment?.Snippet?.TextOriginal}";

                var comments = await restApi.GetCommentsFromThread(commentThread.Id, "snippet,replies",
                    CommentThreadsResource.ListRequest.TextFormatEnum.PlainText.ToString(), null);
                for (var i = 0; i < comments.Items.Count; i++)
                {
                    var commentsItem = comments.Items[i];
                    editor.Text += $"\n{index}.{i} {commentsItem.Snippet.TextOriginal}";
                }
            }
        }
    }
}