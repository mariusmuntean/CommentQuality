using CommentQuality.Core.Interfaces;
using CommentQuality.Core.Models;
using CommentQuality.Core.Services;
using CommentQuality.Core.Stuff;
using System;
using System.Linq;
using System.Threading.Tasks;
#if __WASM__
using Uno.UI.Wasm;
#endif
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CommentQuality.Uno
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
#if __WASM__
            var restApi = new RestApi(new WasmHttpHandler());
#else
            var restApi = new RestApi();
#endif

            var commentThreadIterator = new CommentThreadIterator(restApi);
            var commentThreadProvider = new CommentThreadProvider(commentThreadIterator);

            var commentProcessor = new CommentProcessor(restApi, commentThreadProvider, NewSentimentAnalyzer(restApi));
            var timeBeforeAnalysis = DateTime.UtcNow;
            var averagesSummed = 0.0d;
            var processedCommentsCount = 0;
            await commentProcessor.ProcessCommentsAsync(VideoIdTxtBox.Text, (CommentBatchResult commentBatchResult) =>
            {
                var docsInBatch = commentBatchResult.DocumentBatchSentiment.Documents.Count;
                var sentimentAverage = commentBatchResult.DocumentBatchSentiment.Documents.Sum((arg) => arg.Score);
                var batchSentiment = sentimentAverage / docsInBatch;

                averagesSummed += sentimentAverage;
                processedCommentsCount += commentBatchResult.DocumentBatchSentiment.Documents.Count;

                var msg = $"Processed {processedCommentsCount} comments. Current batch score is {batchSentiment}";
                AppendTextAndScroll(msg).GetAwaiter().GetResult();
            }).ConfigureAwait(true);

            var duration = DateTime.UtcNow.Subtract(timeBeforeAnalysis);
            await Task.Delay(500);
            await AppendTextAndScroll($"Average sentiment score is {(averagesSummed / processedCommentsCount):0.####}");
            await Task.Delay(500);
            await AppendTextAndScroll($"Sentiment analysis took {duration.TotalSeconds:0.##} seconds");
        }

        private static IDocumentBatchSentimentAnalyzer NewSentimentAnalyzer(RestApi restApi)
        {
            //return new DummyDocumentBatchSentimentAnalyzer();

            //return new AzureCognitiveServicesDocumentBatchSentimentAnalyzer(restApi);
            return new GoogleCloudLanguageServiceDocumentBatchSentimentAnalyzer(restApi);
        }

        private async Task AppendTextAndScroll(string text)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () => { OutputTextBlock.Text += $"{Environment.NewLine} {text}"; });
        }
    }
}