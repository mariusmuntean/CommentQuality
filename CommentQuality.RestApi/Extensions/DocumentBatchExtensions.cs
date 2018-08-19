using CommentQuality.RestApi.Models;
using System.Text;

namespace CommentQuality.RestApi.Extensions
{
    public static class DocumentBatchExtensions
    {
        public static readonly string DocumentSeparator = "4C0025AD-F567-4205-8E16-9B3B30788294";

        /// <summary>
        /// CReturns a plain-text representation of a <see cref="DocumentBatch"/> where the text of each document is prefixed with a separator and the document ID
        /// </summary>
        /// <param name="documentBatch"></param>
        /// <returns></returns>
        public static string ToAnnotatedPlainText(this DocumentBatch documentBatch)
        {
            var stringBuilder = new StringBuilder();
            foreach (var document in documentBatch.Documents)
            {
                stringBuilder.AppendLine($"{DocumentSeparator}.");
                stringBuilder.AppendLine($"{document.Id}.");
                stringBuilder.AppendLine(document.Text);
            }

            return stringBuilder.ToString();
        }
    }
}