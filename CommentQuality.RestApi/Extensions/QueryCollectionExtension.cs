using Microsoft.AspNetCore.Http;

namespace CommentQuality.RestApi.Extensions
{
    public static class QueryCollectionExtension
    {
        public static string GetQueryParamValue(this IQueryCollection collection, string key)
        {
            string value = null;
            if (collection.ContainsKey(key))
            {
                value = collection[key];
            }

            return value;
        }
    }
}