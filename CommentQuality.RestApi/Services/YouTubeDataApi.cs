using CommentQuality.RestApi.Interfaces;
using CommentQuality.RestApi.Models;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;

namespace CommentQuality.RestApi.Services
{
    public class YouTubeDataApi : IYouTubeDataApi
    {
        private readonly YouTubeApiSettings _youtubeApiSetting;
        private readonly YouTubeService _youTubeService;

        public YouTubeDataApi(YouTubeApiSettings youtubeApiSetting)
        {
            _youtubeApiSetting = youtubeApiSetting;

            _youTubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = _youtubeApiSetting.ApiKey,
                ApplicationName = "CommentQuality"
            });
        }

        public CommentThreadsResource.ListRequest GetCommentThreadListRequest(string part, string videoId,
            string pageToken)
        {
            var listReq = _youTubeService.CommentThreads.List(part);
            listReq.VideoId = videoId;
            listReq.PageToken = pageToken;

            return listReq;
        }

        public CommentsResource.ListRequest GetCommentListRequest(string part, string parentId,
            CommentsResource.ListRequest.TextFormatEnum textFormat, string pageToken)
        {
            var commentListReq = _youTubeService.Comments.List("snippet");
            commentListReq.ParentId = parentId;
            commentListReq.TextFormat = textFormat;
            commentListReq.PageToken = pageToken;

            return commentListReq;
        }

        public VideosResource.ListRequest GetVideoCommentCount(string videoId)
        {
            var videoStatsListRequest = _youTubeService.Videos.List("statistics");
            videoStatsListRequest.Id = videoId;

            return videoStatsListRequest;
        }
    }
}