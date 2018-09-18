using System;

namespace CommentQuality.Core.Models
{
    public class NotInitializedException : Exception
    {
        public NotInitializedException(string message) : base(message)
        {
        }
    }
}