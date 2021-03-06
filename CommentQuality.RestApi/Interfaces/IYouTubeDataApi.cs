﻿using Google.Apis.YouTube.v3;

namespace CommentQuality.RestApi.Interfaces
{
    public interface IYouTubeDataApi
    {
        /// <summary>
        /// Creates a ListRequest for the Comments Threads of a video.
        /// </summary>
        /// <param name="part">A comma-separated list of commentThread resource properties the the API response should include</param>
        /// <param name="videoId">Identifies the video</param>
        /// <param name="pageToken">Identifies a specific page in the result set.</param>
        /// <returns>The comment thread list request.</returns>
        CommentThreadsResource.ListRequest GetCommentThreadListRequest(string part, string videoId, string pageToken);

        /// <summary>
        /// Creates a ListRequest for the Comments in a CommentThread of a video
        /// </summary>
        /// <param name="part">A comma-separated list of commentThread resource properties the the API response should include</param>
        /// <param name="parentId">The ID of the comment for whom replies shall be retrieved</param>
        /// <param name="textFormat">Indicates whether the API should retrun the comments as HTML or plain text</param>
        /// <param name="pageToken">Identifies a specific page in the result set.</param>
        /// <returns>The comment list request.</returns>
        CommentsResource.ListRequest GetCommentListRequest(string part, string parentId, CommentsResource.ListRequest.TextFormatEnum textFormat, string pageToken);

        VideosResource.ListRequest GetVideoCommentCount(string videoId);
    }
}