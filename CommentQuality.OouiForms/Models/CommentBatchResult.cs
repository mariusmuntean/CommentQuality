namespace CommentQuality.OouiForms.Models
{
    public class CommentBatchResult
    {
        public int TotalCommentCount
        {
            get;
            set;
        }

        public int ProcessedCommentCount
        {
            get;
            set;
        }
        
        public DocumentBatchSentiment DocumentBatchSentiment
        {
            get;
            set;
        }
    }
}
