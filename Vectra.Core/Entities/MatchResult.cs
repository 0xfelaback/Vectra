using System.ComponentModel.DataAnnotations;

namespace Vectra.Core.Entities
{
    /// <summary>
    /// CORE-DM-002: MatchResult Entity
    /// Represents the outcome of a matching attempt between internal and external transactions.
    /// </summary>
    public class MatchResult
    {
        public Guid Id { get; set; }
        public string InternalTransactionReference { get; set; } = string.Empty;
        public string ExternalTransactionReference { get; set; } = string.Empty;
        [Range(0.0, 1.0, ErrorMessage = "ConfidenceScore must be between 0.0 and 1.0.")]
        public decimal ConfidenceScore { get; set; } // Enforced 0.0–1.0 via business logic
        public MatchStrategy MatchingStrategy { get; set; }
        public MatchStatus Status { get; set; }
        public DateTimeOffset MatchedAt { get; set; }
        public string? Reason { get; set; } // reason match was made or why it failed

        // These 5 component weights tell auditors exactly how the composite ConfidenceScore was derived.
        public decimal AmountSignalScore { get; set; }
        public decimal DateProximitySignalScore { get; set; }
        public decimal ReferenceCodeSignalScore { get; set; }
        public decimal DescriptionSimilaritySignalScore { get; set; }
        public decimal CounterpartySignalScore { get; set; }

        /// <summary>
        /// Represents the status of a specific transaction matching attempt.
        /// </summary>
        public enum MatchStatus
        {
            Confirmed = 1,
            Probable,
            Possible,
            Unmatched,
            Duplicate,
            ManuallyResolved
        }


        public enum MatchStrategy
        {
            ExactMatch = 1, ReferenceAnchoredFuzzymatch, AmountDateFuzzyMatch, PartialAmountMatch, RuleBasedMatch
        }
    }
}
