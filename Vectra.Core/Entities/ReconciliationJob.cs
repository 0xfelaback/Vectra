namespace Vectra.Core.Entities
{
    /// <summary>
    /// CORE-DM-003: ReconciliationJob Entity
    /// Represents a single execution run of the reconciliation engine.
    /// </summary>
    public class ReconciliationJob
    {
        public Guid Id { get; set; }
        public JobStatus Status { get; set; }

        // Date range being reconciled
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        // Ingestion metrics
        public int InternalRecordsIngestedCount { get; set; }
        public int ExternalRecordsIngestedCount { get; set; }

        // Match metrics
        public int ConfirmedMatchesCount { get; set; }
        public int ProbableMatchesCount { get; set; }
        public int UnmatchedRecordsCount { get; set; }
        // Executions
        public List<ReconciliationException> ExceptionsEncountered { get; set; } = new();

        // Timing metrics
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }

        // Calculated property for duration
        public TimeSpan Duration => EndTime.HasValue
            ? EndTime.Value - StartTime
            : DateTimeOffset.UtcNow - StartTime;


        /// <summary>
        /// Represents the execution state of a reconciliation job.
        /// </summary>
        public enum JobStatus
        {
            Queued = 1,
            Running,
            Completed,
            Failed,
            PartiallyCompleted
        }
    }
}
