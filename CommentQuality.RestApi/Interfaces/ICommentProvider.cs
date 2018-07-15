using Google.Apis.YouTube.v3.Data;

namespace CommentQuality.RestApi.Interfaces
{
    /// <summary>
    /// Provides comments for a certain YouTube video
    /// </summary>
    interface ICommentProvider
    {
        /// <summary>
        /// Gets the next comment. Calling this method repeatedly will return a new <see cref="Comment"/> every time or null if no more comments are available.
        /// </summary>
        /// <returns>A <see cref="Comment"/> instance or null if no more comments are available</returns>
        Comment GetNextComment();

        /// <summary>
        /// The next available comment. Calling this method repeatedly always returns the same instance of <see cref="Comment"/> or null.
        /// </summary>
        /// <returns></returns>
        Comment PeekNextComment { get; }

        /// <summary>
        /// Checks if any more comments are available.
        /// </summary>
        /// <returns>True if there are more comments, False otherwise</returns>
        bool HasNextComment();

        /// <summary>
        /// Initializes this comment provider to target a certain YouTube video
        /// </summary>
        /// <param name="videoId">The ID of the YouTube video to target</param>
        void Init(string videoId);
    }
}