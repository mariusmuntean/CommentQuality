﻿using System;

namespace CommentQuality.OouiForms.Models
{
    public class BatchedCommentsProviderConfig
    {
        public int MaxDocumentCountPerbatch { get; }
        public int MaxTotalDocumentTextLengthPerBatch { get; }
        public Func<Document, bool> DocumentPredicate { get; }

        public BatchedCommentsProviderConfig(int maxDocumentCountPerbatch, int maxTotalDocumentTextLengthPerBatch, Func<Document, bool> documentPredicate = null)
        {
            MaxDocumentCountPerbatch = maxDocumentCountPerbatch;
            MaxTotalDocumentTextLengthPerBatch = maxTotalDocumentTextLengthPerBatch;
            DocumentPredicate = documentPredicate;
        }
    }
}
