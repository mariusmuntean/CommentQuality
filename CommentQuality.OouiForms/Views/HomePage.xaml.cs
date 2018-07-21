using CommentQuality.OouiForms.Models;
using CommentQuality.OouiForms.Services;
using CommentQuality.OouiForms.Stuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.Immutable;

namespace CommentQuality.OouiForms.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private void ClearClicked(object sender, EventArgs e)
        {
            editor.Text = string.Empty;
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

            await PublishCommentDocuments().ConfigureAwait(true);
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

            int minWorker, minIo;
            ThreadPool.GetMinThreads(out minWorker, out minIo);
            ThreadPool.SetMinThreads(4, minIo);

            for (int i = 0; i < 10; i++)
            {
                var newTask = Task.Run(() =>
                {
                    var commentIterator = new CommentIterator(restApi);
                    var commentProvider2 = new CommentProvider2(commentThreadProvider, commentIterator);

                    var docBatchProvider2 = new DocumentBatchProvider2(
                        new BatchedCommentsProviderConfig(20, 10000,
                            document => !string.IsNullOrWhiteSpace(document.Text)),
                        commentProvider2
                    );

                    DocumentBatch docBatch = docBatchProvider2.GetNextDocumentBatch();
                    while (docBatch.Documents.Any())
                    {
                        foreach (var docBatchDocument in docBatch.Documents)
                        {
                            //AppendTextAndScroll($"{commentCount} - {docBatchDocument.Text}");

                            Interlocked.Increment(ref commentCount);
                        }

                        AppendTextAndScroll($"Comment count = {commentCount}");

                        docBatch = docBatchProvider2.GetNextDocumentBatch();
                    }
                    Console.WriteLine($"{Task.CurrentId} ended");
                });
                tasks.Add(newTask);
            }

            await Task.WhenAll(tasks).ConfigureAwait(true);
            var countDuration = DateTime.Now.Subtract(dateTimeBeforeCounting);
            AppendTextAndScroll($"\nFinal comment count = {commentCount}, took {countDuration.TotalSeconds} seconds");
        }

        private void AppendTextAndScroll(string text)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                editor.Text += $"{Environment.NewLine} {text}";
            });
        }
    }
}