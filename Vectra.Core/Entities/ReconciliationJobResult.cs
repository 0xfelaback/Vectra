
namespace Vectra.Core.Entities
{/// <summary>
 /// Represents the completed evaluation output from an in-memory execution of the core matching engine.
 /// </summary>
    public class ReconciliationJobResult
    {
        /// <summary>
        /// Gets the unique identifier of the execution job context.
        /// </summary>
        public Guid JobId { get; init; }

        /// <summary>
        /// Gets all confirmed, probable, and possible matching pairs found by the engine strategies.
        /// </summary>
        public IReadOnlyCollection<MatchResult> MatchedPairs { get; init; } = Array.Empty<MatchResult>();

        /// <summary>
        /// Gets the list of structured data exceptions, duplicates, and unmatched anomalies found.
        /// </summary>
        public IReadOnlyCollection<ReconciliationException> Exceptions { get; init; } = Array.Empty<ReconciliationException>();

        /// <summary>
        /// Gets the dynamically generated metadata summary report for this evaluation pass.
        /// </summary>
        public ReconciliationReport ExecutionReport { get; init; } = null!;
    }
}
