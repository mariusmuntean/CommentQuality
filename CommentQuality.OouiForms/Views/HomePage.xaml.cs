using CommentQuality.OouiForms.Models;
using CommentQuality.OouiForms.Services;
using CommentQuality.OouiForms.Stuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            //var restApi = new RestApi();
            //var threadListResponse = await restApi.GetCommentThreads(entry.Text, "snippet,replies", null).ConfigureAwait(true);

            //var totalCommentCount = 0;
            //for (var index = 0; index < threadListResponse.Items.Count; index++)
            //{
            //    var commentThread = threadListResponse.Items[index];
            //    editor.Text += $"\n{index} {commentThread.Snippet?.TopLevelComment?.Snippet?.TextOriginal}";
            //    totalCommentCount++;

            //    var comments = await restApi.GetCommentsFromThread(commentThread.Id, "snippet,replies",
            //        CommentThreadsResource.ListRequest.TextFormatEnum.PlainText.ToString(), null).ConfigureAwait(true);
            //    if (comments == null)
            //    {
            //        continue;
            //    }
            //    for (var i = 0; i < comments.Items.Count; i++)
            //    {
            //        var commentsItem = comments.Items[i];
            //        editor.Text += $"\n{index}.{i} {commentsItem.Snippet.TextOriginal}";
            //        totalCommentCount++;
            //    }
            //}

            //editor.Text += $"\n Total comment count = {totalCommentCount}";

            await PublishCommentDocuments().ConfigureAwait(false);
        }

        public async Task PublishCommentDocuments()
        {
            var restApi = new RestApi();
            var commentThreadIterator = new CommentThreadIterator(restApi);
            var commentThreadProvider = new CommentThreadProvider(commentThreadIterator);
            commentThreadProvider.Init(entry.Text, "snippet,replies");

            var tasks = new List<Task>();
            int commentCount = 0;
            var dateTimeBeforeCounting = DateTime.Now;
            for (int i = 0; i < 100; i++)
            {
                var newTask = Task.Run(() =>
                {
                    var commentIterator = new CommentIterator(restApi);
                    var commentProvider2 = new CommentProvider2(commentThreadProvider, commentIterator);

                    var docBatchProvider2 = new DocumentBatchProvider2(
                        new BatchedCommentsProviderConfig(10, 10000,
                            document => !string.IsNullOrWhiteSpace(document.Text)),
                        commentProvider2
                    );

                    DocumentBatch docBatch;
                    while ((docBatch = docBatchProvider2.GetNextDocumentBatch()).Documents.Any())
                    {
                        foreach (var docBatchDocument in docBatch.Documents)
                        {
                            editor.Text += $"\n{docBatchDocument.Id} - {docBatchDocument.Text}";
                            Interlocked.Increment(ref commentCount);
                        }

                        editor.Text += $"\nComment count = {commentCount}";
                    }
                });
                tasks.Add(newTask);
            }

            await Task.WhenAll(tasks).ConfigureAwait(true);
            var countDuration = DateTime.Now.Subtract(dateTimeBeforeCounting);
            editor.Text += $"\nFinal comment count = {commentCount}, took {countDuration.TotalSeconds} seconds";
        }
    }
}