using Google.Apis.YouTube.v3.Data;

namespace CommentQuality.Core.Interfaces
{
    /// <summary>
    /// Provides the <see cref="CommentThread"/>s of a YouTube video
    /// </summary>
    public interface ICommentThreadProvider
    {
        /// <summary>
        /// Initializes the provider to retrieve the <see cref="CommentThread"/>s of a certain video
        /// </summary>
        /// <param name="videoId"></param>
        /// <param name="part"></param>
        void Init(string videoId, string part);

        /// <summary>
        /// Gets the next available <see cref="CommentThread"/> or null if none is available anymore
        /// </summary>
        /// <returns></returns>
        CommentThread GetNextCommentThread();
    }
}