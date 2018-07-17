using System;

namespace CommentQuality.OouiForms.Models
{
    public class NotInitializedException : Exception
    {
        public NotInitializedException(string message) : base(message)
        {
        }
    }
}