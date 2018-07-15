using System;

namespace CommentQuality.RestApi.Models
{
    public class NotInitializedException : Exception
    {
        public NotInitializedException(string message) : base(message)
        {
        }
    }
}