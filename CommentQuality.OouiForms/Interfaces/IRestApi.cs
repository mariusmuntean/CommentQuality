using Google.Apis.YouTube.v3.Data;
using System.Threading.Tasks;

namespace CommentQuality.OouiForms.Interfaces
{
    public interface IRestApi
    {
        Task<string> GetCommentCount(string videoId);

        Task<CommentThreadListResponse> GetCommentThreads(string videoId, string part, string pageToken);

        Task<CommentListResponse> GetCommentsFromThread(string parentId,
            string part,
            string textFormat,
            string pageToken);
    }
}