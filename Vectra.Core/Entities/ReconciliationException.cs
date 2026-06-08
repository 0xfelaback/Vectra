namespace Vectra.Core.Entities
{


    public class ReconciliationException
    {
        public Guid ExceptionId { get; set; } = Guid.NewGuid();
        public Guid JobId { get; set; }
        public ReconciliationExceptionType Type { get; set; }
        public List<Guid> AffectedTransactionIds { get; set; } = new();
        public string Description { get; set; } = string.Empty;
        public string SuggestedResolutionAction { get; set; } = string.Empty;
        public DateTimeOffset ClassifiedAt { get; set; } = DateTimeOffset.UtcNow;
        public bool IsResolved { get; set; }

        public enum ReconciliationExceptionType
        {
            Unmatched,
            ProbableMatchAwaitingReview,
            DuplicateDetected,
            AmbiguousMatch,
            CurrencyMismatch
        }
    }
}