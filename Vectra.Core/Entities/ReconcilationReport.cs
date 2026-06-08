namespace Vectra.Core.Entities
{
    public class ReconciliationReport
    {
        public Guid ReportId { get; set; } = Guid.NewGuid();
        public Guid JobId { get; set; }
        public decimal MatchRate { get; set; }
        public int TotalRecordsProcessed => IngestedInternalCount + IngestedExternalCount;
        public int IngestedInternalCount { get; set; }
        public int IngestedExternalCount { get; set; }
        public int ConfirmedMatchesCount { get; set; }
        public int ProbableMatchesCount { get; set; }
        public int UnmatchedRecordsCount { get; set; }
        public Dictionary<string, int> ExceptionTypeDistribution { get; set; } = new();
        public DateTimeOffset GeneratedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
